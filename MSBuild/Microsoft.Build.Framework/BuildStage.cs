//
// BuildStage.cs: Build stage enum
//
// Author:
//   Rob Tillie (Rob@flep-tech.nl)
// 
// (C) Rob Tillie

namespace Microsoft.Build.Framework
{
	public enum BuildStage
	{
		NotBuilding,
		BuildStarted,
		BuildFinished,
		ProjectStarted,
		ProjectFinished,
		TargetStarted,
		TargetFinished,
		TaskStarted,
		TaskFinished
	}
}
