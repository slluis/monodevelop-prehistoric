using GLib;
using Gtk;
using GtkSharp;
using System;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Runtime.InteropServices;
using Mono.Debugger;
using Mono.Debugger.Languages;

using MonoDevelop.Core.Services;
using MonoDevelop.Internal.Parser;
using MonoDevelop.Services;

using RefParse = ICSharpCode.SharpRefactory.Parser;
using AST = ICSharpCode.SharpRefactory.Parser.AST;

namespace MonoDevelop.Debugger
{
	public class DebuggerVariablePad : Gtk.ScrolledWindow
	{
		Mono.Debugger.StackFrame current_frame;

		Gtk.TreeView tree;
		Gtk.TreeStore store;
		bool is_locals_display;

		const int NAME_COL = 0;
		const int VALUE_COL = 1;
		const int TYPE_COL = 2;
		const int RAW_VIEW_COL = 3;

		public DebuggerVariablePad (bool is_locals_display)
		{
			this.ShadowType = ShadowType.In;

			this.is_locals_display = is_locals_display;

			store = new TreeStore (typeof (string),
					       typeof (string),
			                       typeof (string),
					       typeof (bool));

			tree = new TreeView (store);
			tree.RulesHint = true;
			tree.HeadersVisible = true;

			TreeViewColumn NameCol = new TreeViewColumn ();
			CellRenderer NameRenderer = new CellRendererText ();
			NameCol.Title = "Name";
			NameCol.PackStart (NameRenderer, true);
			NameCol.AddAttribute (NameRenderer, "text", NAME_COL);
			NameCol.Resizable = true;
			NameCol.Alignment = 0.0f;
			tree.AppendColumn (NameCol);

			TreeViewColumn ValueCol = new TreeViewColumn ();
			CellRenderer ValueRenderer = new CellRendererText ();
			ValueCol.Title = "Value";
			ValueCol.PackStart (ValueRenderer, true);
			ValueCol.AddAttribute (ValueRenderer, "text", VALUE_COL);
			ValueCol.Resizable = true;
			NameCol.Alignment = 0.0f;
			tree.AppendColumn (ValueCol);

			TreeViewColumn TypeCol = new TreeViewColumn ();
			CellRenderer TypeRenderer = new CellRendererText ();
			TypeCol.Title = "Type";
			TypeCol.PackStart (TypeRenderer, true);
			TypeCol.AddAttribute (TypeRenderer, "text", TYPE_COL);
			TypeCol.Resizable = true;
			NameCol.Alignment = 0.0f;
			tree.AppendColumn (TypeCol);

			tree.TestExpandRow += new TestExpandRowHandler (test_expand_row);

			Add (tree);
			ShowAll ();

			DebuggingService dbgr = (DebuggingService)ServiceManager.GetService (typeof (DebuggingService));
			dbgr.PausedEvent += new EventHandler (OnPausedEvent);
			dbgr.ResumedEvent += new EventHandler (OnResumedEvent);
			dbgr.StoppedEvent += new EventHandler (OnStoppedEvent);
		}

		bool add_array (TreeIter parent, ITargetArrayObject array)
		{
			bool inserted = false;

			for (int i = array.LowerBound; i < array.UpperBound; i++) {
				ITargetObject elt = array [i];
				if (elt == null)
					continue;

				TreeIter iter = store.Append (parent);
				add_object (elt, i.ToString (), iter);
				inserted = true;
			}

			return inserted;
		}

		bool add_member (TreeIter parent, ITargetStructObject sobj, ITargetMemberInfo member, bool is_field)
		{
			bool inserted = false;

#if NET_2_0
			DebuggerBrowsableAttribute battr = GetDebuggerBrowsableAttribute (member);
			if (battr == null) {
				TreeIter iter = store.Append (parent);
				add_object (is_field ? sobj.GetField (member.Index) : sobj.GetProperty (member.Index),
					    member.Name, iter);
				inserted = true;
			}
			else {
				TreeIter iter;

				switch (battr.State) {
				case DebuggerBrowsableState.Never:
					// don't display it at all
					continue;
				case DebuggerBrowsableState.Collapsed:
					// the default behavior for the debugger (c&p from above)
					iter = store.Append (parent);
					add_object (is_field ? sobj.GetField (member.Index) : sobj.GetProperty (member.Index),
						    member.Name, iter);
					inserted = true;
					break;
				case DebuggerBrowsableState.Expanded:
					// add it as in the Collapsed case...
					iter = store.Append (parent);
					add_object (is_field ? sobj.GetField (member.Index) : sobj.GetProperty (member.Index),
						    member.Name, iter);
					inserted = true;
					// then expand the row
					tree.ExpandRow (store.GetPath (iter), false);
					break;
				case DebuggerBrowsableState.RootHidden:
					ITargetObject member_obj = is_field ? sobj.GetField (member.Index) : sobj.GetProperty (member.Index);

					if (member_obj != null) {
						switch (member_obj.TypeInfo.Type.Kind) {
						case TargetObjectKind.Array:
							iter = store.Append (parent);
							// handle arrays normally, should check how vs2005 does this.
							add_object (member_obj, member.Name, iter);
							inserted = true;
							break;
						case TargetObjectKind.Class:
							try {
								add_class (parent, (ITargetClassObject)member_obj, false);
								inserted = true;
							}
							catch {
								// what about this case?  where the member is possibly
								// uninitialized, do we try to add it later?
							}
							break;
						case TargetObjectKind.Struct:
							try {
								add_struct (parent, (ITargetStructObject)member_obj, false);
								inserted = true;
							}
							catch {
								// what about this case?  where the member is possibly
								// uninitialized, do we try to add it later?
							}
							break;
						default:
							// nothing
							break;
						}
					}
					break;
				}
			}
#else
			TreeIter iter = store.Append (parent);
			add_object (sobj.GetField (member.Index), member.Name, iter);
			inserted = true;
#endif

			return inserted;
		}

		bool add_struct (TreeIter parent, ITargetStructObject sobj, bool raw_view)
		{
			bool inserted = false;

#if NET_2_0
			if (!raw_view) {
				DebuggingService dbgr = (DebuggingService)ServiceManager.GetService (typeof (DebuggingService));
				DebuggerTypeProxyAttribute pattr = GetDebuggerTypeProxyAttribute (dbgr, sobj);

				if (pattr != null) {
					Mono.Debugger.StackFrame frame = dbgr.MainThread.CurrentFrame;
	 				ITargetStructType proxy_type = frame.Language.LookupType (frame, pattr.ProxyTypeName) as ITargetStructType;
					if (proxy_type == null)
						proxy_type = frame.Language.LookupType (frame,
											sobj.Type.Name + "+" + pattr.ProxyTypeName) as ITargetStructType;
					if (proxy_type != null) {
						string name = String.Format (".ctor({0})", sobj.Type.Name);
						ITargetMethodInfo method = null;

						foreach (ITargetMethodInfo m in proxy_type.Constructors) {
							Console.WriteLine ("   " + m.FullName);
							if (m.FullName == name)
								method = m;
						}

						if (method != null) {
							ITargetFunctionObject ctor = proxy_type.GetConstructor (frame, method.Index);
							ITargetObject[] args = new ITargetObject[1];
							args[0] = sobj;

							ITargetStructObject proxy_obj = ctor.Type.InvokeStatic (frame, args, false) as ITargetStructObject;

							if (proxy_obj != null) {
								foreach (ITargetPropertyInfo prop in proxy_obj.Type.Properties) {
									if (add_member (parent, proxy_obj, prop, false))
										inserted = true;
								}

								TreeIter iter = store.Append (parent);
								store.SetValue (iter, NAME_COL, new GLib.Value ("Raw View"));
								store.SetValue (iter, RAW_VIEW_COL, new GLib.Value (true));
								add_data (sobj, iter);

								return true;
							}
						}
					}
				}
			}
#endif

			foreach (ITargetFieldInfo field in sobj.Type.Fields) {
				if (add_member (parent, sobj, field, true))
					inserted = true;
			}

			foreach (ITargetPropertyInfo prop in sobj.Type.Properties) {
				if (add_member (parent, sobj, prop, false))
					inserted = true;
			}

			return inserted;
		}

		bool add_class (TreeIter parent, ITargetClassObject sobj, bool raw_view)
		{
			bool inserted = false;

			if (sobj.Type.HasParent) {
				TreeIter iter = store.Append (parent);
				add_object (sobj.Parent, "<parent>", iter);
				inserted = true;
			}

			if (add_struct (parent, sobj, raw_view))
				inserted = true;

			return inserted;
		}

		void add_message (TreeIter parent, string message)
		{
			TreeIter child;
			if (store.IterChildren (out child, parent)) {
				while (!(child.Equals (Gtk.TreeIter.Zero)) && (child.Stamp != 0))
					store.Remove (ref child);
			}

			TreeIter iter = store.Append (parent);
			store.SetValue (iter, VALUE_COL, new GLib.Value (message));
		}

		void test_expand_row (object o, TestExpandRowArgs args)
		{
			bool inserted = false;

			ITargetObject obj = (ITargetObject) iters [args.Iter];

			TreeIter child;
			if (store.IterChildren (out child, args.Iter)) {
				while (!(child.Equals (Gtk.TreeIter.Zero)) && (child.Stamp != 0))
					store.Remove (ref child);
			}

			if (obj == null) {
				child = store.Append (args.Iter);
				return;
			}

			switch (obj.TypeInfo.Type.Kind) {
			case TargetObjectKind.Array:
				ITargetArrayObject array = (ITargetArrayObject) obj;
				try {
					inserted = add_array (args.Iter, array);
				} catch {
					add_message (args.Iter, "<can't display array>");
					inserted = true;
				}
				if (!inserted)
					add_message (args.Iter, "<empty array>");
				break;

			case TargetObjectKind.Class:
				ITargetClassObject cobj = (ITargetClassObject) obj;
				try {
					bool raw_view = (bool)store.GetValue (args.Iter, RAW_VIEW_COL);
					inserted = add_class (args.Iter, cobj, raw_view);
				} catch (Exception e) {
				  Console.WriteLine (e);
					add_message (args.Iter, "<can't display class>");
					inserted = true;
				}
				if (!inserted)
					add_message (args.Iter, "<empty class>");
				break;

			case TargetObjectKind.Struct:
				ITargetStructObject sobj = (ITargetStructObject) obj;
				try {
					bool raw_view = (bool)store.GetValue (args.Iter, RAW_VIEW_COL);
					inserted = add_struct (args.Iter, sobj, raw_view);
				} catch {
					add_message (args.Iter, "<can't display struct>");
					inserted = true;
				}
				if (!inserted)
					add_message (args.Iter, "<empty struct>");
				break;

			default:
				add_message (args.Iter, "<unknown object>");
				break;
			}
		}

		void add_data (ITargetObject obj, TreeIter parent)
		{
			/*TreeIter iter = */ store.Append (parent);
			iters.Add (parent, obj);
		}

#if NET_2_0
		DebuggerBrowsableAttribute GetDebuggerBrowsableAttribute (ITargetMemberInfo info)
		{
	  		if (info.Handle != null && info.Handle is System.Reflection.MemberInfo) {
				System.Reflection.MemberInfo mi = (System.Reflection.MemberInfo)info.Handle;
				object[] attrs = mi.GetCustomAttributes (typeof (DebuggerBrowsableAttribute), false);

				if (attrs != null && attrs.Length > 0)
					return (DebuggerBrowsableAttribute)attrs[0];
			}

			return null;
		}

		DebuggerTypeProxyAttribute GetDebuggerTypeProxyAttribute (DebuggingService dbgr, ITargetObject obj)
		{
			if (obj.TypeInfo.Type.TypeHandle != null && obj.TypeInfo.Type.TypeHandle is Type)
				return dbgr.AttributeHandler.GetDebuggerTypeProxyAttribute ((Type)obj.TypeInfo.Type.TypeHandle);

			return null;
		}

		DebuggerDisplayAttribute GetDebuggerDisplayAttribute (DebuggingService dbgr, ITargetObject obj)
		{
			if (obj.TypeInfo.Type.TypeHandle != null && obj.TypeInfo.Type.TypeHandle is Type)
			  return dbgr.AttributeHandler.GetDebuggerDisplayAttribute ((Type)obj.TypeInfo.Type.TypeHandle);

			return null;
		}

		string EvaluateDebuggerDisplay (ITargetObject obj, string display)
		{
			StringBuilder sb = new StringBuilder ("");
			DebuggingService dbgr = (DebuggingService)ServiceManager.GetService (typeof (DebuggingService));
			EvaluationContext ctx = new EvaluationContext (obj);

			ctx.CurrentProcess = new ProcessHandle (dbgr.MainThread);

			/* break up the string into runs of {...} and
			 * normal text.  treat the {...} as C#
			 * expressions, and evaluate them */
			int start_idx = 0;

			while (true) {
				int left_idx;
				int right_idx;
				left_idx = display.IndexOf ('{', start_idx);

				if (left_idx == -1) {
					/* we're done. */
					sb.Append (display.Substring (start_idx));
					break;
				}
				if (left_idx != start_idx) {
					sb.Append (display.Substring (start_idx, left_idx - start_idx));
				}
				right_idx = display.IndexOf ('}', left_idx + 1);
				if (right_idx == -1) {
					// '{...\0'.  ignore the '{', append the rest, and break out */
					sb.Append (display.Substring (left_idx + 1));
					break;
				}

				if (right_idx - left_idx > 1) {
					/* there's enough space for an
					 * expression.  parse it and see
					 * what we get. */
					RefParse.Parser parser;
					AST.Expression ast_expr;
					Expression dbgr_expr;
					DebuggerASTVisitor visitor;
					string snippet;
					object retval;

					/* parse the snippet to build up MD's AST */
					parser = new RefParse.Parser();

					snippet = display.Substring (left_idx + 1, right_idx - left_idx - 1);
					ast_expr = parser.ParseExpression (new RefParse.Lexer (new RefParse.StringReader (snippet)));

					/* use our visitor to convert from MD's AST to types that
					 * facilitate evaluation by the debugger */
					visitor = new DebuggerASTVisitor ();
					dbgr_expr = (Expression)ast_expr.AcceptVisitor (visitor, null);

					/* finally, resolve and evaluate the expression */
					dbgr_expr = dbgr_expr.Resolve (ctx);
					retval = dbgr_expr.Evaluate (ctx);

#region "c&p'ed from debugger/frontend/Style.cs"
					if (retval is long) {
						sb.Append (String.Format ("0x{0:x}", (long) retval));
					}
					else if (retval is string) {
						sb.Append ('"' + (string) retval + '"');
					}
					else if (retval is ITargetObject) {
						ITargetObject tobj = (ITargetObject) retval;
						sb.Append (tobj.Print ());
					}
					else {
						sb.Append (retval.ToString ());
					}
#endregion
				}

				start_idx = right_idx + 1;
			}

			return sb.ToString ();
		}
#endif

		void add_object (ITargetObject obj, string name, TreeIter iter)
		{
			AmbienceService amb = (AmbienceService)MonoDevelop.Core.Services.ServiceManager.GetService (typeof (AmbienceService));

			store.SetValue (iter, NAME_COL, new GLib.Value (name));
			if (obj == null) {
				store.SetValue (iter, VALUE_COL, new GLib.Value ("null"));
				return;
			}
			store.SetValue (iter, TYPE_COL, new GLib.Value (amb.CurrentAmbience.GetIntrinsicTypeName (obj.TypeInfo.Type.Name)));

			switch (obj.TypeInfo.Type.Kind) {
			case TargetObjectKind.Fundamental:
				object contents = ((ITargetFundamentalObject) obj).Object;
				store.SetValue (iter, VALUE_COL, new GLib.Value (contents.ToString ()));
				break;
			case TargetObjectKind.Array:
				add_data (obj, iter);
				break;
			case TargetObjectKind.Struct:
			case TargetObjectKind.Class:
#if NET_2_0
				try {
					DebuggingService dbgr = (DebuggingService)ServiceManager.GetService (typeof (DebuggingService));
					DebuggerDisplayAttribute dattr = GetDebuggerDisplayAttribute (dbgr, obj);
					if (dattr != null) {
						store.SetValue (iter, VALUE_COL,
								new GLib.Value (EvaluateDebuggerDisplay (obj, dattr.Value)));
					}
					else {
						// call the object's ToString() method and stuff that in the Value field
						store.SetValue (iter, VALUE_COL,
								new GLib.Value (((ITargetStructObject)obj).PrintObject()));
					}
				}
				catch (Exception e) {
					Console.WriteLine ("getting object value failed: {0}", e);

					store.SetValue (iter, VALUE_COL,
							new GLib.Value (""));
				}
#endif
				add_data (obj, iter);
				break;
			}
		}

		void add_variable (IVariable variable)
		{
			if (!variable.IsAlive (current_frame.TargetAddress))
				return;

			TreeIter iter;
			store.Append (out iter);

			try {
				ITargetObject obj = variable.GetObject (current_frame);
				add_object (obj, variable.Name, iter);
			} catch (LocationInvalidException) {
				// Do nothing
			} catch (Exception e) {
				Console.WriteLine ("CAN'T ADD VARIABLE: {0} {1}", variable, e);
			}
		}

		Hashtable iters = null;

		public void CleanDisplay ()
		{
			store.Clear ();
			iters = new Hashtable ();
		}

		public void UpdateDisplay ()
		{
			CleanDisplay ();

			if ((current_frame == null) || (current_frame.Method == null))
				return;

			try {
				if (is_locals_display) {
					IVariable[] local_vars = current_frame.Method.Locals;
					foreach (IVariable var in local_vars)
						add_variable (var);
				} else {
					IVariable[] param_vars = current_frame.Method.Parameters;
					foreach (IVariable var in param_vars)
						add_variable (var);
				}
			} catch (Exception e) {
				Console.WriteLine ("CAN'T GET VARIABLES: {0}", e);
				store.Clear ();
				iters = new Hashtable ();
			}
		}

		protected void OnStoppedEvent (object o, EventArgs args)
		{
			CleanDisplay ();
		}

		protected void OnResumedEvent (object o, EventArgs args)
		{
			CleanDisplay ();
		}

		protected void OnPausedEvent (object o, EventArgs args)
		{
			DebuggingService dbgr = (DebuggingService)ServiceManager.GetService (typeof (DebuggingService));
			current_frame = (Mono.Debugger.StackFrame)dbgr.CurrentFrame;
			UpdateDisplay ();
		}
	}
}
