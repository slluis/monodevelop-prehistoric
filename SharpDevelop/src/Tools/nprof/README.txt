nprof v0.3 release
------------------

THIS IS AN ALPHA RELEASE.

To run:
  1. Register the nprof.hook.dll file by running RegisterProfilerHook.bat
  2. Start NProf.Application.exe
  3. Click the browse button to find a .NET executable.
  4. Click the Start button to start profiling!
  
To use VS.NET addin:
  1. Register the VS.NET addin by running RegisterVSNetAddin.bat
  2. Start VS.NET
  3. Open a solution
  4. Click the "Enable NProf" button to start profiling
  5. You may need to disable/enable nprof between runs
  
Known bugs:
  - VS.NET steals focus from the add-in window
  - Occasionally, a socket error occurs for an unknown reason
  
A test application to profile is included: TestProfilee.exe
    
Problems?  Email mmastrac@canada.com

  
  
