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
using MonoDevelop.Services;

using Debugger.Frontend;

namespace MonoDevelop.SourceEditor.Gui
{
	public class DebuggerVariablePad : Gtk.ScrolledWindow
	{
		Mono.Debugger.StackFrame current_frame;

		Gtk.TreeView tree;
		Gtk.TreeStore store;
		bool is_locals_display;

		public DebuggerVariablePad (bool is_locals_display)
		{
			this.ShadowType = ShadowType.In;

			this.is_locals_display = is_locals_display;

			store = new TreeStore (typeof (string),
					       typeof (string),
			                       typeof (string));

			tree = new TreeView (store);
			tree.RulesHint = true;
			tree.HeadersVisible = true;

			TreeViewColumn NameCol = new TreeViewColumn ();
			CellRenderer NameRenderer = new CellRendererText ();
			NameCol.Title = "Name";
			NameCol.PackStart (NameRenderer, true);
			NameCol.AddAttribute (NameRenderer, "text", 0);
			NameCol.Resizable = true;
			NameCol.Alignment = 0.0f;
			tree.AppendColumn (NameCol);

			TreeViewColumn TypeCol = new TreeViewColumn ();
			CellRenderer TypeRenderer = new CellRendererText ();
			TypeCol.Title = "Type";
			TypeCol.PackStart (TypeRenderer, true);
			TypeCol.AddAttribute (TypeRenderer, "text", 1);
			TypeCol.Resizable = true;
			NameCol.Alignment = 0.0f;
			tree.AppendColumn (TypeCol);

			TreeViewColumn ValueCol = new TreeViewColumn ();
			CellRenderer ValueRenderer = new CellRendererText ();
			ValueCol.Title = "Value";
			ValueCol.PackStart (ValueRenderer, true);
			ValueCol.AddAttribute (ValueRenderer, "text", 2);
			ValueCol.Resizable = true;
			NameCol.Alignment = 0.0f;
			tree.AppendColumn (ValueCol);

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
								add_class (parent, (ITargetClassObject)member_obj);
								inserted = true;
							}
							catch {
								// what about this case?  where the member is possibly
								// uninitialized, do we try to add it later?
							}
							break;
						case TargetObjectKind.Struct:
							try {
								add_struct (parent, (ITargetStructObject)member_obj);
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

		bool add_struct (TreeIter parent, ITargetStructObject sobj)
		{
			bool inserted = false;

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

		bool add_class (TreeIter parent, ITargetClassObject sobj)
		{
			bool inserted = false;

			if (sobj.Type.HasParent) {
				TreeIter iter = store.Append (parent);
				add_object (sobj.Parent, "<parent>", iter);
				inserted = true;
			}

			if (add_struct (parent, sobj))
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
			store.SetValue (iter, 2, new GLib.Value (message));
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
					inserted = add_class (args.Iter, cobj);
				} catch {
					add_message (args.Iter, "<can't display class>");
					inserted = true;
				}
				if (!inserted)
					add_message (args.Iter, "<empty class>");
				break;

			case TargetObjectKind.Struct:
				ITargetStructObject sobj = (ITargetStructObject) obj;
				try {
					inserted = add_struct (args.Iter, sobj);
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

		DebuggerDisplayAttribute GetDebuggerDisplayAttribute (ITargetObject obj)
		{
			if (obj.TypeInfo.Type.TypeHandle != null && obj.TypeInfo.Type.TypeHandle is Type) {
				Type t = (Type)obj.TypeInfo.Type.TypeHandle;
				object[] attrs = t.GetCustomAttributes (typeof (DebuggerDisplayAttribute), false);

				if (attrs != null && attrs.Length > 0)
					return (DebuggerDisplayAttribute)attrs[0];
			}

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
					// there's enough space for an
					// expression.  parse it and see
					// what we get.

					string snippet = display.Substring (left_idx + 1, right_idx - left_idx - 1);

					CSharpExpressionParser parser = new CSharpExpressionParser (ctx, snippet);
					Expression expr = parser.Parse (snippet);

					expr = expr.Resolve (ctx);
					object retval = expr.Evaluate (ctx);

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
			store.SetValue (iter, 0, new GLib.Value (name));
			store.SetValue (iter, 1, new GLib.Value (amb.CurrentAmbience.GetIntrinsicTypeName (obj.TypeInfo.Type.Name)));

			switch (obj.TypeInfo.Type.Kind) {
			case TargetObjectKind.Fundamental:
				object contents = ((ITargetFundamentalObject) obj).Object;
				store.SetValue (iter, 2, new GLib.Value (contents.ToString ()));
				break;

			case TargetObjectKind.Array:
				add_data (obj, iter);
				break;
			case TargetObjectKind.Struct:
			case TargetObjectKind.Class:
#if NET_2_0
				DebuggerDisplayAttribute dattr = GetDebuggerDisplayAttribute (obj);
				if (dattr != null)
					store.SetValue (iter, 2,
							new GLib.Value (EvaluateDebuggerDisplay (obj, dattr.Value)));
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
