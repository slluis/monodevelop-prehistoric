//
// IBuildEngine.cs: build engine interface.
//
// Author:
//   Rob Tillie (Rob@flep-tech.nl)
// 
// (C) Rob Tillie

using System;
using System.Collections;

namespace Microsoft.Build.Framework
{
    public interface IBuildEngine
    {
        bool BuildProjectFile (string projectFileName, string[] targetNames, IDictionary targetOutputs);
        void LogBuildEvent (TaskEventArgs e);
    }
}
