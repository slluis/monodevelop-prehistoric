using System;

//
// ITask.cs: Interface for a task.
//
// Author:
//   Rob Tillie (Rob@flep-tech.nl)
// 
// (C) Rob Tillie

namespace Microsoft.Build.Framework
{
    public interface ITask
    {
        bool Execute ();

        IBuildEngine BuildEngine 
        {
            get;
            set;
        }

        object HostObject 
        {
            get;
            set;
        }
    }
}
