// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Lluis Sanchez Gual" email="lluis@ximian.com"/>
//     <version value="$version"/>
// </file>


using System;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Activation;

namespace MonoDevelop.Services
{
	public class SyncContext
	{
		[ThreadStatic]
		static SyncContext context;
		
		static Hashtable delegateFactories = new Hashtable ();
		static ModuleBuilder module;
		static AssemblyBuilder asmBuilder;
	
		public static void SetContext (SyncContext ctx)
		{
			context = ctx;
		}
		
		public static SyncContext GetContext ()
		{
			return context;
		}
		
		public virtual void Dispatch (StatefulMessageHandler cb, object ob)
		{
			cb (ob);
		}
		
		public virtual void AsyncDispatch (StatefulMessageHandler cb, object ob)
		{
			cb.BeginInvoke (ob, null, null);
		}
		
		public Delegate CreateSynchronizedDelegate (Delegate del)
		{
			lock (delegateFactories.SyncRoot)
			{
				Type delType = del.GetType();
				IDelegateFactory factory = delegateFactories [delType] as IDelegateFactory;
				if (factory == null)
				{
					Type t = GetDelegateFactoryType (delType);
					factory = Activator.CreateInstance (t) as IDelegateFactory;
					delegateFactories [delType] = factory;
				}
				return factory.Create (del, this);
			}
		}
			
		Type GetDelegateFactoryType (Type delegateType)
		{
			MethodInfo invoke = delegateType.GetMethod ("Invoke");
			ModuleBuilder module = GetModuleBuilder ();
			TypeBuilder typeBuilder = module.DefineType ("__" + delegateType.Name + "_DelegateFactory", TypeAttributes.Public, typeof(object), new Type[] {typeof(IDelegateFactory)});
			
			// Context and target delegate field
			
			FieldBuilder contextField = typeBuilder.DefineField ("context", typeof(SyncContext), FieldAttributes.Public);
			FieldBuilder targetField = typeBuilder.DefineField ("target", delegateType, FieldAttributes.Public);
			
			// Parameters
			
			ParameterInfo[] pars = invoke.GetParameters ();
			FieldBuilder[] paramFields = new FieldBuilder [pars.Length];
			Type[] paramTypes = new Type[pars.Length];
			for (int n=0; n<pars.Length; n++)
			{
				ParameterInfo pi = pars [n];
				paramFields [n] = typeBuilder.DefineField ("p" + n, pi.ParameterType, FieldAttributes.Public);
				paramTypes [n] = pi.ParameterType;
			}
			
			// Return value
			
			FieldBuilder returnField = null;
			if (invoke.ReturnType != typeof(void))
				returnField = typeBuilder.DefineField ("ret", invoke.ReturnType, FieldAttributes.Public);
			
			// Constructor
			
			ConstructorBuilder ctor = typeBuilder.DefineConstructor (MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
			ConstructorInfo baseCtor = typeof(object).GetConstructor (Type.EmptyTypes);
			ILGenerator gen = ctor.GetILGenerator();
			gen.Emit (OpCodes.Ldarg_0);
			gen.Emit (OpCodes.Call, baseCtor);
			gen.Emit (OpCodes.Ret);
			
			// Dispatch method
			
			MethodBuilder methodDispatch = typeBuilder.DefineMethod ("Dispatch", MethodAttributes.Public, typeof(void), new Type[] {typeof(object)});
			gen = methodDispatch.GetILGenerator();
			if (returnField != null)
				gen.Emit (OpCodes.Ldarg_0);
			gen.Emit (OpCodes.Ldarg_0);
			gen.Emit (OpCodes.Ldfld, targetField);
			
			for (int n=0; n<pars.Length; n++)
			{
				gen.Emit (OpCodes.Ldarg_0);
				gen.Emit (OpCodes.Ldfld, paramFields[n]);
			}
			gen.Emit (OpCodes.Callvirt, invoke);
			
			if (returnField != null)
				gen.Emit (OpCodes.Stfld, returnField);
	
			gen.Emit (OpCodes.Ret);
			
			// ProxyCall method
			
			MethodBuilder methodProxyCall = typeBuilder.DefineMethod ("ProxyCall", MethodAttributes.Public, invoke.ReturnType, paramTypes);
			gen = methodProxyCall.GetILGenerator();
			for (int n=0; n<paramFields.Length; n++) {
				gen.Emit (OpCodes.Ldarg_0);
				gen.Emit (OpCodes.Ldarg, n+1);
				gen.Emit (OpCodes.Stfld, paramFields[n]);
			}
			gen.Emit (OpCodes.Ldarg_0);
			gen.Emit (OpCodes.Ldfld, contextField);
			gen.Emit (OpCodes.Ldarg_0);
			gen.Emit (OpCodes.Ldftn, methodDispatch);
			gen.Emit (OpCodes.Newobj, typeof(StatefulMessageHandler).GetConstructor (new Type[] {typeof(object), typeof(IntPtr)} ));
			gen.Emit (OpCodes.Ldnull);
			gen.Emit (OpCodes.Callvirt, typeof(SyncContext).GetMethod ("Dispatch"));
			
			if (returnField != null) {
				gen.Emit (OpCodes.Ldarg_0);
				gen.Emit (OpCodes.Ldfld, returnField);
			}
			gen.Emit (OpCodes.Ret);
			
			// Create method
			
			MethodBuilder methodCreate = typeBuilder.DefineMethod ("Create", MethodAttributes.Public, typeof(Delegate), new Type[] {typeof(Delegate), typeof(SyncContext)});
			gen = methodCreate.GetILGenerator();
			LocalBuilder vthis = gen.DeclareLocal (typeBuilder);
			gen.Emit (OpCodes.Newobj, ctor);
			gen.Emit (OpCodes.Stloc, vthis);
			gen.Emit (OpCodes.Ldloc, vthis);
			gen.Emit (OpCodes.Ldarg_1);
			gen.Emit (OpCodes.Castclass, delegateType);
			gen.Emit (OpCodes.Stfld, targetField);
			gen.Emit (OpCodes.Ldloc, vthis);
			gen.Emit (OpCodes.Ldarg_2);
			gen.Emit (OpCodes.Stfld, contextField);
			gen.Emit (OpCodes.Ldloc, vthis);
			gen.Emit (OpCodes.Ldftn, methodProxyCall);
			gen.Emit (OpCodes.Newobj, delegateType.GetConstructor (new Type[] {typeof(object), typeof(IntPtr)} ));
			gen.Emit (OpCodes.Ret);
			typeBuilder.DefineMethodOverride (methodCreate, typeof(IDelegateFactory).GetMethod ("Create"));
			
			return typeBuilder.CreateType ();
		}
		
		static ModuleBuilder GetModuleBuilder ()
		{
			if (module == null)
			{
				AppDomain myDomain = System.Threading.Thread.GetDomain();
				AssemblyName myAsmName = new AssemblyName();
				myAsmName.Name = "MonoDevelop.DelegateGenerator.GeneratedAssembly";
	
				asmBuilder = myDomain.DefineDynamicAssembly (myAsmName, AssemblyBuilderAccess.RunAndSave);
				module = asmBuilder.DefineDynamicModule ("MonoDevelop.DelegateGenerator.GeneratedAssembly", "MonoDevelop.DelegateGenerator.GeneratedAssembly.dll");
			}
			return module;
		}
	}
	
	public interface IDelegateFactory
	{
		Delegate Create (Delegate del, SyncContext ctx);
	}
}
