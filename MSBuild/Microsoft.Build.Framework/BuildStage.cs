//
// Author:
//   Rob Tillie (Rob@flep-tech.nl)
// 

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
