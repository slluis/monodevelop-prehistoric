using System;
using System.Collections;

//
// Author:
//   Rob Tillie (Rob@flep-tech.nl)
// 

namespace Microsoft.Build.Framework
{
    public interface IBuildEngine
    {
        bool BuildProjectFile (string projectFileName, string[] targetNames, IDictionary targetOutputs);
        void LogBuildEvent (TaskEventArgs e);
    }
}
