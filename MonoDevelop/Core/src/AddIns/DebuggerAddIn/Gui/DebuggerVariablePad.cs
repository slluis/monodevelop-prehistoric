using GLib;
using Gtk;
using GtkSharp;
using System;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using Mono.Debugger;
using Mono.Debugger.Languages;

using Stock = MonoDevelop.Gui.Stock;
using MonoDevelop.Core.Services;
using MonoDevelop.Internal.Parser;
using MonoDevelop.Services;

namespace MonoDevelop.Debugger
{
	public class DebuggerVariablePad : Gtk.ScrolledWindow
	{
		Mono.Debugger.StackFrame current_frame;

		Hashtable variable_rows;
		Hashtable iters;

		Gtk.TreeView tree;
		Gtk.TreeStore store;
		bool is_locals_display;

		internal const int NAME_COL = 0;
		internal const int VALUE_COL = 1;
		internal const int TYPE_COL = 2;
		internal const int RAW_VIEW_COL = 3;
		internal const int PIXBUF_COL = 4;

		public DebuggerVariablePad (bool is_locals_display)
		{
			this.ShadowType = ShadowType.In;

			this.is_locals_display = is_locals_display;

			variable_rows = new Hashtable();
			iters = new Hashtable();

			store = new TreeStore (typeof (string),
					       typeof (string),
			                       typeof (string),
					       typeof (bool),
					       typeof (Gdk.Pixbuf));

			tree = new TreeView (store);
			tree.RulesHint = true;
			tree.HeadersVisible = true;

			TreeViewColumn NameCol = new TreeViewColumn ();
			CellRenderer NameRenderer = new CellRendererText ();
			CellRenderer IconRenderer = new CellRendererPixbuf ();
			NameCol.Title = "Name";
			NameCol.PackStart (IconRenderer, false);
			NameCol.PackStart (NameRenderer, true);
			NameCol.AddAttribute (IconRenderer, "pixbuf", PIXBUF_COL);
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

			tree.TestExpandRow += new TestExpandRowHandler (TestExpandRow);

			Add (tree);
			ShowAll ();

			Runtime.DebuggingService.PausedEvent += new EventHandler (OnPausedEvent);
			Runtime.DebuggingService.StoppedEvent += new EventHandler (OnStoppedEvent);
		}

		bool InsertArrayChildren (TreeIter parent, ITargetArrayObject array)
		{
			bool inserted = false;

			for (int i = array.LowerBound; i < array.UpperBound; i++) {

				inserted = true;

				ITargetObject elt = array [i];
				if (elt == null)
					continue;

				TreeIter iter = store.Append (parent);
				AddObject (i.ToString (), "" /* XXX */, elt, iter);
			}

			return inserted;
		}

		bool InsertStructMember (TreeIter parent, ITargetStructObject sobj, ITargetMemberInfo member, bool is_field)
		{
			bool inserted = false;

			string icon_name = GetIcon (member);

#if NET_2_0
			DebuggerBrowsableAttribute battr = GetDebuggerBrowsableAttribute (member);
			if (battr != null) {
				TreeIter iter;

				switch (battr.State) {
				case DebuggerBrowsableState.Never:
					// don't display it at all
					continue;
				case DebuggerBrowsableState.Collapsed:
					// the default behavior for the debugger (c&p from above)
					iter = store.Append (parent);
					AddObject (member.Name, icon_name, is_field ? sobj.GetField (member.Index) : sobj.GetProperty (member.Index),
						   iter);
					inserted = true;
					break;
				case DebuggerBrowsableState.Expanded:
					// add it as in the Collapsed case...
					iter = store.Append (parent);
					AddObject (member.Name, icon_name, is_field ? sobj.GetField (member.Index) : sobj.GetProperty (member.Index),
						   iter);
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
							AddObject (member.Name, icon_name, member_obj, iter);
							inserted = true;
							break;
						case TargetObjectKind.Class:
							try {
								inserted = InsertClassChildren (parent, (ITargetClassObject)member_obj, false);
							}
							catch {
								// what about this case?  where the member is possibly
								// uninitialized, do we try to add it later?
							}
							break;
						case TargetObjectKind.Struct:
							try {
								inserted = InsertStructChildren (parent, (ITargetStructObject)member_obj, false);
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
			else {
#endif
				TreeIter iter = store.Append (parent);
				AddObject (member.Name, icon_name, is_field ? sobj.GetField (member.Index) : sobj.GetProperty (member.Index),
					   iter);
				inserted = true;
#if NET_2_0
			}
#endif

			return inserted;
		}

#if NET_2_0
		bool InsertProxyChildren (DebuggingService dbgr, DebuggerTypeProxyAttribute pattr, TreeIter parent, ITargetStructObject sobj)
		{
			Mono.Debugger.StackFrame frame = dbgr.MainThread.CurrentFrame;
	 		ITargetStructType proxy_type = frame.Language.LookupType (frame, pattr.ProxyTypeName) as ITargetStructType;
			if (proxy_type == null)
				proxy_type = frame.Language.LookupType (frame,
									sobj.Type.Name + "+" + pattr.ProxyTypeName) as ITargetStructType;
			if (proxy_type != null) {
				string name = String.Format (".ctor({0})", sobj.Type.Name);
				ITargetMethodInfo method = null;

				foreach (ITargetMethodInfo m in proxy_type.Constructors) {
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
							InsertStructMember (parent, proxy_obj, prop, false);
						}

						TreeIter iter = store.Append (parent);
						store.SetValue (iter, NAME_COL, "Raw View");
						store.SetValue (iter, RAW_VIEW_COL, true);

						Gdk.Pixbuf icon = Runtime.Gui.Resources.GetIcon (Stock.Class, Gtk.IconSize.Menu);
						if (icon != null)
							store.SetValue (iter, PIXBUF_COL, icon);

						iters.Remove (iter);
						AddPlaceholder (sobj, iter);

						return true;
					}
				}
			}

			return false;
		}
#endif

		bool InsertStructChildren (TreeIter parent, ITargetStructObject sobj, bool raw_view)
		{
			bool inserted = false;

#if NET_2_0
			if (!raw_view) {
				DebuggingService dbgr = (DebuggingService)Runtime.DebuggingService;
				DebuggerTypeProxyAttribute pattr = GetDebuggerTypeProxyAttribute (dbgr, sobj);

				if (pattr != null) {
					if (InsertProxyChildren (dbgr, pattr, parent, sobj))
						inserted = true;
				}
			}
#endif

			foreach (ITargetFieldInfo field in sobj.Type.Fields) {
				if (InsertStructMember (parent, sobj, field, true))
					inserted = true;
			}

			foreach (ITargetPropertyInfo prop in sobj.Type.Properties) {
				if (InsertStructMember (parent, sobj, prop, false))
					inserted = true;
			}

			return inserted;
		}

		bool InsertClassChildren (TreeIter parent, ITargetClassObject sobj, bool raw_view)
		{
			bool inserted = false;

			if (sobj.Type.HasParent) {
				TreeIter iter = store.Append (parent);
				AddObject ("<parent>", Stock.Class, sobj.Parent, iter);
				inserted = true;
			}

			if (InsertStructChildren (parent, sobj, raw_view))
				inserted = true;

			return inserted;
		}

		void InsertMessage (TreeIter parent, string message)
		{
			TreeIter child;
			if (store.IterChildren (out child, parent)) {
				while (!(child.Equals (Gtk.TreeIter.Zero)) && (child.Stamp != 0))
					store.Remove (ref child);
			}

			TreeIter iter = store.Append (parent);
			store.SetValue (iter, VALUE_COL, message);
		}

		void TestExpandRow (object o, TestExpandRowArgs args)
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
					inserted = InsertArrayChildren (args.Iter, array);
				} catch {
					InsertMessage (args.Iter, "<can't display array>");
					inserted = true;
				}
				if (!inserted)
					InsertMessage (args.Iter, "<empty array>");
				break;

			case TargetObjectKind.Class:
				ITargetClassObject cobj = (ITargetClassObject) obj;
				try {
					bool raw_view = (bool)store.GetValue (args.Iter, RAW_VIEW_COL);
					inserted = InsertClassChildren (args.Iter, cobj, raw_view);
				} catch (Exception e) {
				  Console.WriteLine (e);
					InsertMessage (args.Iter, "<can't display class>");
					inserted = true;
				}
				if (!inserted)
					InsertMessage (args.Iter, "<empty class>");
				break;

			case TargetObjectKind.Struct:
				ITargetStructObject sobj = (ITargetStructObject) obj;
				try {
					bool raw_view = (bool)store.GetValue (args.Iter, RAW_VIEW_COL);
					inserted = InsertStructChildren (args.Iter, sobj, raw_view);
				} catch {
					InsertMessage (args.Iter, "<can't display struct>");
					inserted = true;
				}
				if (!inserted)
					InsertMessage (args.Iter, "<empty struct>");
				break;

			default:
				InsertMessage (args.Iter, "<unknown object>");
				break;
			}
		}

		void AddPlaceholder (ITargetObject obj, TreeIter parent)
		{
			if (obj.TypeInfo.Type.Kind == TargetObjectKind.Array) {
				ITargetArrayObject array = (ITargetArrayObject) obj;
				if (array.LowerBound == array.UpperBound)
					return;
			}

			store.Append (parent);
			iters.Add (parent, obj);
		}

		string GetObjectValueString (ITargetObject obj)
		{
			if (obj == null) {
				return "null";
			}

			switch (obj.TypeInfo.Type.Kind) {
			case TargetObjectKind.Fundamental:
				object contents = ((ITargetFundamentalObject) obj).Object;
				return contents.ToString ();

			case TargetObjectKind.Array:
				ITargetArrayObject array = (ITargetArrayObject) obj;
				if (array.LowerBound == array.UpperBound && array.LowerBound == 0)
					return "[]";
				else
					return "";

			case TargetObjectKind.Struct:
			case TargetObjectKind.Class:
				try {
#if NET_2_0
					DebuggingService dbgr = (DebuggingService)Runtime.DebuggingService;
					DebuggerDisplayAttribute dattr = GetDebuggerDisplayAttribute (dbgr, obj);
					if (dattr != null) {
						return dbgr.AttributeHandler.EvaluateDebuggerDisplay (obj, dattr.Value);
					}
					else {
#endif
						// call the object's ToString() method.
						return ((ITargetStructObject)obj).PrintObject();
#if NET_2_0
					}
#endif
				}
				catch (Exception e) {
				  //Console.WriteLine ("getting object value failed: {0}", e);
					return "";
				}
			default:
				return "";
			}
		}

		void AddObject (string name, string icon_name, ITargetObject obj, TreeIter iter)
		{
			store.SetValue (iter, NAME_COL, name);
			store.SetValue (iter, VALUE_COL, GetObjectValueString (obj));
			store.SetValue (iter, TYPE_COL,
					obj == null ? "" : Runtime.Ambience.CurrentAmbience.GetIntrinsicTypeName (obj.TypeInfo.Type.Name));
			Gdk.Pixbuf icon = Runtime.Gui.Resources.GetIcon (icon_name, Gtk.IconSize.Menu);
			if (icon != null)
				store.SetValue (iter, PIXBUF_COL, icon);
			if (obj != null)
				AddPlaceholder (obj, iter);
		}

		string GetIcon (ITargetObject obj)
		{
			string icon = "";

			if (obj.TypeInfo.Type.TypeHandle is Type)
				icon = Runtime.Gui.Icons.GetIcon ((Type)obj.TypeInfo.Type.TypeHandle);

			return icon;
		}

		string GetIcon (ITargetMemberInfo member)
		{
			string icon = "";

			if (member.Handle is PropertyInfo)
				icon = Runtime.Gui.Icons.GetIcon ((PropertyInfo)member.Handle);
			else if (member.Handle is FieldInfo)
				icon = Runtime.Gui.Icons.GetIcon ((FieldInfo)member.Handle);

			return icon;
		}

		void UpdateVariableChildren (IVariable variable, ITargetObject obj, TreePath path, TreeIter iter)
		{
			bool expanded = tree.GetRowExpanded (path);
			TreeIter citer;

			if (!expanded) {

				/* we aren't expanded, just remove all
				 * children and add the object back
				 * (since it might be a different
				 * object now) */

				if (store.IterChildren (out citer, iter))
					while (store.Remove (ref citer)) ;
				iters.Remove (iter);

				AddPlaceholder (obj, iter);
			}
			else {
				/* in a perfect world, we'd just iterate
				 * over the stuff we're showing and update
				 * it.  for now, just remove all rows and
				 * re-add them. */

				if (store.IterChildren (out citer, iter))
					while (store.Remove (ref citer)) ;

				iters.Remove (iter);

				AddObject (variable.Name, GetIcon (obj), obj, iter);

				tree.ExpandRow (path, false);
			}
		}

		void UpdateVariable (IVariable variable)
		{
			TreeRowReference row = (TreeRowReference)variable_rows[variable];

			if (row == null) {
				/* the variable isn't presently displayed */

				if (!variable.IsAlive (current_frame.TargetAddress))
					/* it's not displayed and not alive, just return */
					return;

				AddVariable (variable);
			}
			else {
				/* the variable is presently displayed */

				// XXX we need a obj.IsValid check in this branch

				if (!variable.IsAlive (current_frame.TargetAddress)) {
					/* it's in the display but no longer alive.  remove it */
					RemoveVariable (variable);
					return;
				}

				/* it's still alive - make sure the display is up to date */
				TreeIter iter;
				if (store.GetIter (out iter, row.Path)) {
					try {
						ITargetObject obj = variable.GetObject (current_frame);

						/* make sure the Value column is correct */
						string current_value = (string)store.GetValue (iter, VALUE_COL);
						string new_value = GetObjectValueString (obj);
						if (current_value != new_value)
							store.SetValue (iter, VALUE_COL, new_value);

						/* update the children */
						UpdateVariableChildren (variable, obj, row.Path, iter);

					} catch (Exception e) {
						Console.WriteLine ("can't update variable: {0} {1}", variable, e);
						store.SetValue (iter, VALUE_COL, "");
					}
				}
			}
		}

		void AddVariable (IVariable variable)
		{
			try {
				/* it's alive, add it to the display */

				ITargetObject obj = variable.GetObject (current_frame);
				TreeIter iter;

				if (!obj.IsValid)
					return;

				store.Append (out iter);

				variable_rows.Add (variable, new TreeRowReference (store, store.GetPath (iter)));

				AddObject (variable.Name, GetIcon (obj), obj, iter);
			} catch (LocationInvalidException) {
				// Do nothing
			} catch (Exception e) {
				Console.WriteLine ("can't add variable: {0} {1}", variable, e);
			}
		}

		void RemoveVariable (IVariable variable)
		{
			TreeRowReference row = (TreeRowReference)variable_rows[variable];
			TreeIter iter;

			if (row != null && store.GetIter (out iter, row.Path)) {
				iters.Remove (iter);
				store.Remove (ref iter);
			}

			variable_rows.Remove (variable);
		}

		public void UpdateDisplay ()
		{
			if ((current_frame == null) || (current_frame.Method == null))
				return;

			try {
				Hashtable vars_to_remove = new Hashtable();

				foreach (IVariable var in variable_rows.Keys) {
					vars_to_remove.Add (var, var);
				}

				if (is_locals_display) {
					if (current_frame.Method.HasThis) {
						UpdateVariable (current_frame.Method.This);
						vars_to_remove.Remove (current_frame.Method.This);
					}
					IVariable[] local_vars = current_frame.Method.Locals;
					foreach (IVariable var in local_vars) {
						UpdateVariable (var);
						vars_to_remove.Remove (var);
					}
				} else {
					IVariable[] param_vars = current_frame.Method.Parameters;
					foreach (IVariable var in param_vars) {
						UpdateVariable (var);
						vars_to_remove.Remove (var);
					}
				}

				foreach (IVariable var in vars_to_remove.Keys) {
					RemoveVariable (var);
				}

			} catch (Exception e) {
				Console.WriteLine ("CAN'T GET VARIABLES: {0}", e);
				store.Clear ();
				iters = new Hashtable ();
			}
		}

		protected void OnStoppedEvent (object o, EventArgs args)
		{
			current_frame = (Mono.Debugger.StackFrame)Runtime.DebuggingService.CurrentFrame;
			UpdateDisplay ();
		}

		protected void OnPausedEvent (object o, EventArgs args)
		{
			current_frame = (Mono.Debugger.StackFrame)Runtime.DebuggingService.CurrentFrame;
			UpdateDisplay ();
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
#endif

	}
}
