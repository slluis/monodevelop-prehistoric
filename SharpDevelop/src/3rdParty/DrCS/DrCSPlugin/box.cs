
// DrC# Interactions Component
// created on 9/7/2002
// Written by Mathias Ricken
// Rice University

using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;

using Rice.Drcsharp.Interpreter;

namespace Rice.Drcsharp
{
	/// <summary>
	/// This component is a restricted input/output textbox.
	/// It only allows the user to edit the last line.
	/// </summary>
	public class InteractionsTextBox : TextBox
	{
		/// <summary>
		/// Position of the cursor
		/// </summary>
		int cursor;

		/// <summary>
		/// Current line
		/// </summary>
		String currentLine;

		/// <summary>
		/// Plugin that contains this box
		/// </summary>
		InteractionsPlugin plugin;

		/// <summary>
		/// History of lines
		/// </summary>
		ArrayList history;

		/// <summary>
		/// Current position within the history.
		/// 0 is not in history; 1 is all the way at the end!
		/// </summary>
		int currentHistoryItem;

		/// <summary>
		/// Maximum size of the history
		/// </summary>
		const int MAX_HISTORY_SIZE = 20;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="s">Text to initially put into the box</param>
		/// <param name="p">Plugin that contains this box</param>
		public InteractionsTextBox(String s, InteractionsPlugin p)
		{
			plugin = p;

			Text = s;
			Multiline = true;
			WordWrap = true;
			BackColor = Color.White;
			ForeColor = Color.Black;
			Font = new Font("Courier New", 10);
			ScrollBars = ScrollBars.Vertical;
			currentLine = "";
			cursor = 0;

			// install keyboard handler
			KeyPress += new KeyPressEventHandler(KeyPressed);

			history = new ArrayList();
			currentHistoryItem = 0;
		}

		/// <summary>
		/// Print out information about the current line
		/// </summary>
		void PrintInfo()
		{
			/*
			Console.WriteLine("String = '"+currentLine+"'");
			Console.WriteLine("Length = "+currentLine.Length);
			Console.WriteLine("Cursor = "+cursor);
			*/
		}

		/// <summary>
		/// Override the function that determines which keys
		/// are considered input for the text box.
		/// Disable cursor keys, shift-insert/delete,
		/// control-C/X/V, and delete
		/// </summary>
		/// <param name="keyData">Key to check</param>
		/// <returns>true if key is input</returns>
		protected override bool IsInputKey(Keys keyData)
		{
			if (keyData==Keys.Left ||
				keyData==Keys.Right ||
				keyData==Keys.Up ||
				keyData==Keys.Down ||
				keyData==(Keys.Shift | Keys.Insert) ||
				keyData==(Keys.Shift | Keys.Delete) ||
				keyData==(Keys.Control | Keys.C) ||
				keyData==(Keys.Control | Keys.X) ||
				keyData==(Keys.Control | Keys.V) ||
				keyData==Keys.Delete) {
				return false;
			} else {
				return base.ProcessDialogKey(keyData);
			}
		}

		/// <summary>
		/// Process special keys
		/// </summary>
		/// <param name="keyData">Key pressed</param>
		/// <returns>true if the key has been handled</returns>
		protected override bool ProcessDialogKey(Keys keyData)
		{
			bool bRet = false;

			// Hack : two different / key codes !?!
			if ((int)keyData == 131263)
			{
				keyData = Keys.Divide | Keys.Control;
			}

			if (keyData==Keys.Left)
			{
				// Console.WriteLine("left");
				ScrollToEnd();
				if (cursor<1)
				{
					// swallow
					bRet = true;
					cursor = 0;
				}
				else
				{
					// do not swallow
					--cursor;
				}
				PrintInfo();
			}
			else if (keyData==Keys.Right)
			{
				// Console.WriteLine("right");
				ScrollToEnd();
				if (cursor>=currentLine.Length)
				{
					// swallow
					bRet = true;
					cursor = currentLine.Length;
				}
				else
				{
					// do not swallow
					++cursor;
				}
				PrintInfo();
			}
			else if (keyData==Keys.Up)
			{
				// Console.WriteLine("up");
				// swallow
				bRet = true;

				GetPreviousHistoryItem();
			}
			else if (keyData==Keys.Down)
			{
				// Console.WriteLine("down");
				// swallow
				bRet = true;

				GetNextHistoryItem();
			}
			else if (keyData==(Keys.Shift | Keys.Insert) ||
					 keyData==(Keys.Control | Keys.V))
			{
				// Console.WriteLine("paste");

				// TODO: only works if there is no editor window open
				//		 #D steals this key from us!

				ScrollToEnd();

				// TODO: pasting in general not implemented

				// swallow
				bRet = true;
			}
			else if (keyData==(Keys.Shift | Keys.Delete) ||
					 keyData==(Keys.Control | Keys.X))
			{
				// Console.WriteLine("cut");

				// TODO: only works if there is no editor window open
				//		 #D steals this key from us!

				// swallow
				bRet = true;
				if (SelectionStart<Text.Length-currentLine.Length ||
					SelectionStart+SelectionLength<Text.Length-currentLine.Length)
				{
					// cannot cut, not in edit line
				}
				else
				{
					currentLine = currentLine.Remove(SelectionStart-(Text.Length-currentLine.Length),SelectionLength);
					Cut();
				}
			}
			else if (keyData==(Keys.Control | Keys.C))
			{
				// Console.WriteLine("copy");

				// TODO: only works if there is no editor window open
				//		 #D steals this key from us!
				Copy();

				// swallow
				bRet = true;
			}
			else if (keyData==Keys.Delete)
			{
				// Console.WriteLine("delete");
				ScrollToEnd();
				if (cursor>=currentLine.Length)
				{
					// swallow
					bRet = true;
					cursor = currentLine.Length;
				}
				else
				{
					// do not swallow
					currentLine = currentLine.Remove(cursor,1);
				}
			}
			else
			{
				bRet = base.ProcessDialogKey(keyData);
			}

			return bRet;
		}

		/// <summary>
		/// Scroll to where the cursor should be
		/// </summary>
		void ScrollToEnd()
		{
			SelectionLength = 0;
			SelectionStart = Text.Length - currentLine.Length + cursor;
			//ScrollToCaret(); //ia this needed???
		}

		public void ReinitializeText() {
			Text = "Welcome to DrC#";
			PrintNewCaret();
		}
		
		public void PrintNewCaret() {
			Text += Environment.NewLine + "> ";
			cursor = 0;
			currentLine = "";
			ScrollToEnd();
			ScrollToCaret();
		}

		/// <summary>
		/// Keyboard handler
		/// </summary>
		/// <param name="sender">Message sender</param>
		/// <param name="e">Event arguments</param>
		public void KeyPressed(object sender, KeyPressEventArgs e)
		{
			ScrollToEnd();

			// get the ASCII code
			char ch = e.KeyChar;

			// did the user press RETURN?
			if ((int)ch==13)
			{
				if (currentLine!="")
				{
					// add input to history
					AddToHistory(currentLine);

					string tempCopy = String.Copy(currentLine);

					// clear the current line
					currentLine = "";
					cursor = 0;

					// write newline and "> " prompt
					PrintNewCaret();
					
					// handle the input and prints only if the input produced a return value
					string returnResult = plugin.InterpretInteractions(tempCopy);
					if (returnResult != "") {
						Write(returnResult + Environment.NewLine);
					}






					ScrollToEnd();
					ScrollToCaret();

					// swallow the event
					e.Handled = true;
				}
				else
				{
					// do not allow empty lines
					// swallow event
					e.Handled = true;
				}
			}
			else if ((int)ch==8)
			{
				if (currentLine.Length==0 || cursor==0)
				{
					// swallow the event
					e.Handled = true;
				}
				else
				{
					// make the current string shorter
					if (cursor==currentLine.Length)
					{
						currentLine = currentLine.Substring(0,currentLine.Length-1);
					}
					else
					{
						currentLine = currentLine.Remove(cursor-1,1);
					}
					--cursor;

					// do not swallow the event
				}
				PrintInfo();
			}
			else
			{
				// add to the current string
				if (cursor>=currentLine.Length)
				{
					currentLine += ch;
				}
				else
				{
					currentLine = currentLine.Insert(cursor,""+ch);
				}
				++cursor;

				PrintInfo();
				// do not swallow the event
			}
		}


		/// <summary>
		/// Adds text in the correct place
		/// </summary>
		/// <param name="text">Text to add</param>
		public void Write(String text)
		{
			// first, delete our current string and the "> " marker
			if (Text.Length>=currentLine.Length+2)
			{
				Text = Text.Substring(0,Text.Length-currentLine.Length-2);
			}
			else
			{
				Text = "";
			}

			text += "> "+currentLine;

			try
			{
				// try to append it using the preferred way
				AppendText(text);

			}
			catch(System.ArgumentOutOfRangeException e)
			{
				e = e;

				// if it fails, just add the text
				Text += text;
				// db("AppendText Exception: " + e);
			}
		}

		/// <summary>
		/// Add a string to the history
		/// </summary>
		/// <param name="s">String to add</param>
		void AddToHistory(String s)
		{
			if (history.Count>=MAX_HISTORY_SIZE)
			{
				history.RemoveRange(0,history.Count-MAX_HISTORY_SIZE+1);
			}
			history.Add(s);
			currentHistoryItem = 0;
		}

		/// <summary>
		/// Get the previous item from the history and
		/// make it the current line
		/// </summary>
		void GetPreviousHistoryItem()
		{
			if (currentHistoryItem<history.Count)
			{
				// first, delete our current string and the "> " marker
				if (Text.Length>=currentLine.Length+2)
				{
					Text = Text.Substring(0,Text.Length-currentLine.Length-2);
				}
				else
				{
					Text = "";
				}

				++currentHistoryItem;
				currentLine = (String)history[history.Count-currentHistoryItem];
				cursor = currentLine.Length;

				String text = "> "+currentLine;

				try
				{
					// try to append it using the preferred way
					AppendText(text);
				}
				catch(System.ArgumentOutOfRangeException e)
				{
					e = e;

					// if it fails, just add the text
					Text += text;
					// db("AppendText Exception: " + e);
				}
			}
		}

		/// <summary>
		/// Get the next item from the history and
		/// make it the current line
		/// </summary>
		void GetNextHistoryItem()
		{
			if (currentHistoryItem==0)
			{
				return;
			}

			// first, delete our current string and the "> " marker
			if (Text.Length>=currentLine.Length+2)
			{
				Text = Text.Substring(0,Text.Length-currentLine.Length-2);
			}
			else
			{
				Text = "";
			}

			if (currentHistoryItem>1)
			{
				--currentHistoryItem;
				currentLine = (String)history[history.Count-currentHistoryItem];
			}
			else
			{
				currentHistoryItem = 0;
				currentLine = "";
			}

			cursor = currentLine.Length;

			String text = "> "+currentLine;

			try
			{
				// try to append it using the preferred way
				AppendText(text);
			}
			catch(System.ArgumentOutOfRangeException e)
			{
				e = e;

				// if it fails, just add the text
				Text += text;
				// db("AppendText Exception: " + e);
			}
		}
	}
}
