using System;

//
// Author:
//   Rob Tillie (Rob@flep-tech.nl)
// 

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
