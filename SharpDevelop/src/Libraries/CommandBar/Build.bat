@echo off
%SystemRoot%\Microsoft.net\Framework\v1.1.4322\csc.exe /target:library /out:CommandBar.dll Library\*.cs %1
%SystemRoot%\Microsoft.net\Framework\v1.1.4322\csc.exe /target:exe /out:Example.exe Example\Example.cs /r:CommandBar.dll %1
