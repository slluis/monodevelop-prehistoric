//
// BuildStatusEventArgs.cs: event arguments for build status events
//
// Author:
//   Rob Tillie (Rob@flep-tech.nl)
// 
// (C) Rob Tillie

using System;

namespace Microsoft.Build.Framework
{
	public sealed class BuildStatusEventArgs : BuildEventArgs
	{
		private BuildStage stage;
		private string stageName;
		private bool success;

		public BuildStatusEventArgs ()
		{
			this.Category = BuildEventCategory.Custom;
			this.Importance = BuildEventImportance.Normal;
		}

		public BuildStatusEventArgs (BuildStage stage)
		{
			this.Category = BuildEventCategory.Custom;
			this.Importance = BuildEventImportance.Normal; 
			this.stage = stage;
		}

		public BuildStatusEventArgs (BuildStage stage, string stageName)
		{
			this.Category = BuildEventCategory.Custom;
			this.Importance = BuildEventImportance.Normal;
			this.stage = stage;
			this.stageName = stageName;
		}

		public BuildStatusEventArgs (BuildStage stage, bool success)
		{
			this.Category = BuildEventCategory.Custom;
			this.Importance = BuildEventImportance.Normal;
			this.stage = stage;
			this.success = success;
		}

		public BuildStatusEventArgs (BuildStage stage, string stageName, bool success)
		{
			this.Category = BuildEventCategory.Custom;
			this.Importance = BuildEventImportance.Normal;
			this.stage = stage;
			this.stageName = stageName;
			this.success = success;
		}

		public bool IsStageSuccessfullyFinished {
			get { return success; }
			set { success = value; }
		}

		public BuildStage Stage {
			get { return stage; }
			set { stage = value; }
		}

		public string StageName {
			get { return stageName; }
			set { stageName = value; }
		}
	}
}
