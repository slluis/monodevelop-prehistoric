using System;
using NUnit.Framework;
using Microsoft.Build.Framework;

namespace Microsoft.Build.Framework.Test {
	
	[TestFixture]
	public class EventArgsTest {

		private BuildEventArgs e;
		private BuildStatusEventArgs be;
		private TaskEventArgs te;

		[SetUp]
		public void Init () 
		{
			e = new BuildEventArgs ();
			be = new BuildStatusEventArgs ();
			te = new TaskEventArgs ();
		}

		[TearDown]
		public void CleanUp () 
		{
			
		}
		
		[Test]
		public void BuildEventArgsTest ()
		{
			// the timestamp should be set to DateTime.Now
			Assert.AreEqual (DateTime.Now.Day, e.TimeStamp.Day, "VAL#01");
			Assert.AreEqual (0, e.Processor, "VAL#02");
			Assert.AreEqual (BuildEventCategory.Comment, e.Category, "VAL#03");
			Assert.IsNull (e.SubCategory, "VAL#04");
			Assert.AreEqual (BuildEventImportance.Low, e.Importance, "VAL#05");
			Assert.IsNull (e.Code, "VAL#06");
			Assert.IsNull (e.File, "VAL#07");
			Assert.AreEqual (0, e.LineNumber, "VAL#08");
			Assert.AreEqual (0, e.ColumnNumber, "VAL#09");
			Assert.AreEqual (0, e.EndLineNumber, "VAL#10");
			Assert.AreEqual (0, e.EndColumnNumber, "VAL#11");
			Assert.IsNull (e.Message, "VAL#12");
			Assert.IsNull (e.HelpKeyword, "VAL#13");
		}

		[Test]
		public void BuildStatusEventArgsTest ()
		{
			// Include these again because the inheriter could change these in the constructor.
			Assert.AreEqual (DateTime.Now.Day, be.TimeStamp.Day, "BSE#01");
			Assert.AreEqual (0, be.Processor, "BSE#02");
			Assert.AreEqual (BuildEventCategory.Custom, be.Category, "BSE#03");
			Assert.IsNull (be.SubCategory, "BSE#04");
			Assert.AreEqual (BuildEventImportance.Normal, be.Importance, "BSE#05");
			Assert.IsNull (be.Code, "BSE#06");
			Assert.IsNull (be.File, "BSE#07");
			Assert.AreEqual (0, be.LineNumber, "BSE#08");
			Assert.AreEqual (0, be.ColumnNumber, "BSE#09");
			Assert.AreEqual (0, be.EndLineNumber, "BSE#10");
			Assert.AreEqual (0, be.EndColumnNumber, "BSE#11");
			Assert.IsNull (be.Message, "BSE#12");
			Assert.IsNull (be.HelpKeyword, "BSE#13");

			Assert.AreEqual (false, be.IsStageSuccessfullyFinished, "BSE#14");
			Assert.AreEqual (null, be.StageName, "BSE#15");
			Assert.AreEqual (BuildStage.NotBuilding, be.Stage, "BSE#16");
		}

		[Test]
		public void TaskEventArgsTest ()
		{
			// Include these again because the inheriter could change these in the constructor.
			Assert.AreEqual (DateTime.Now.Day, te.TimeStamp.Day, "TEA#01");
			Assert.AreEqual (0, te.Processor, "TEA#02");
			Assert.AreEqual (BuildEventCategory.Comment, te.Category, "TEA#03");
			Assert.IsNull (te.SubCategory, "TEA#04");
			Assert.AreEqual (BuildEventImportance.Low, te.Importance, "TEA#05");
			Assert.IsNull (te.Code, "TEA#06");
			Assert.IsNull (te.File, "TEA#07");
			Assert.AreEqual (0, te.LineNumber, "TEA#08");
			Assert.AreEqual (0, te.ColumnNumber, "TEA#09");
			Assert.AreEqual (0, te.EndLineNumber, "TEA#10");
			Assert.AreEqual (0, te.EndColumnNumber, "TEA#11");
			Assert.IsNull (te.Message, "TEA#12");
			Assert.IsNull (te.HelpKeyword, "TEA#13");

			Assert.IsNull (te.TaskName, "TEA#14");
		}
	}
}
