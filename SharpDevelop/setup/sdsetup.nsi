; SharpDevelop installer
; Date: 2003-10-23

!define MUI_PRODUCT "SharpDevelop"
!define MUI_VERSION "0.98"

!include "MUI.nsh"

;--------------------------------
;Configuration

 !define MUI_WELCOMEPAGE
 !define MUI_LICENSEPAGE
 !define MUI_DIRECTORYPAGE
 !define MUI_ABORTWARNING

 !define MUI_FINISHPAGE
 !define MUI_TEXT_FINISH_RUN "Run #develop"
 !define MUI_FINISHPAGE_RUN "$INSTDIR\bin\SharpDevelop.exe"

 !define MUI_UNINSTALLER
 !define MUI_UNCONFIRMPAGE

 !insertmacro MUI_LANGUAGE "English"

 ; !define MUI_ICON "..\data\resources\SharpDevelop.ico"
 ; !define MUI_SPECIALBITMAP "wizard-image.bmp"
 !define MUI_BRANDINGTEXT "© 2003 ic#code, http://www.icsharpcode.net/"


Name "SharpDevelop"
Caption "SharpDevelop Installer"
OutFile SharpDevelop.exe

LicenseText "You must read the following license before installing:"
LicenseData "..\doc\license.txt"

WindowIcon on
; Icon ..\data\resources\SharpDevelop.ico

CRCCheck on

InstProgressFlags smooth
AutoCloseWindow false
ShowInstDetails hide ; or show

ShowUninstDetails nevershow
DirText "Please select a location to install program files (or use the default):"
SetOverwrite on ; use ifnewer if overwrite is a problem
SetDateSave on

InstallDir $PROGRAMFILES\SharpDevelop


; .NET Framework check
; http://msdn.microsoft.com/netframework/default.aspx?pull=/library/en-us/dnnetdep/html/redistdeploy1_1.asp
; Section "Detecting that the .NET Framework 1.1 is installed"

Function .onInit
	ReadRegDWORD $R0 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v1.1.4322" Install
	StrCmp $R0 "" 0 CheckPreviousVersion
	MessageBox MB_OK "Microsoft .NET Framework 1.1 was not found on this system.$\r$\n$\r$\nUnable to continue this installation."
	Abort

  CheckPreviousVersion:
	ReadRegStr $R0 HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\SharpDevelop" DisplayName
	StrCmp $R0 "" NoAbort 0
	MessageBox MB_OK "An old version of SharpDevelop is installed on this computer, please uninstall first.$\r$\n$\r$\nUnable to continue this installation."
	Abort

  NoAbort:
FunctionEnd


; Copy all base files
Section "Copying all required files"

  ; Copy all files
  SetOutPath $INSTDIR
  File /r ..\AddIns
  File /r ..\bin
  File /r ..\data
  File /r ..\doc

SectionEnd

; Start menu stuff
Section "Start Menu + Desktop Icons"
  SetOutPath $SMPROGRAMS\SharpDevelop
  CreateShortCut "$SMPROGRAMS\SharpDevelop\SharpDevelop.lnk" \
                 "$INSTDIR\bin\SharpDevelop.exe"
  CreateShortCut "$SMPROGRAMS\SharpDevelop\SharpUnit.lnk" \
                 "$INSTDIR\bin\SharpUnit.exe"
  CreateShortCut "$SMPROGRAMS\SharpDevelop\SharpDevelop Help.lnk" \
                 "$INSTDIR\doc\help\sharpdevelop.chm"
  CreateShortCut "$SMPROGRAMS\SharpDevelop\Uninstall SharpDevelop.lnk" \
                 "$INSTDIR\uninst.exe"

SectionEnd

; Runs after other programs are finished installing
Section -post
  ; Set the user path
  ; ReadRegStr $R0 HKCU Environment path
  ; NOT USED in .94 ONWARDS
  ; WriteRegStr HKCU Environment path "$R0;$INSTDIR\bin"

  ; Write Registry settings for Add/Remove
  WriteRegStr HKLM SOFTWARE\SharpDevelop "" $INSTDIR
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\SharpDevelop" \
                   DisplayName "SharpDevelop (remove only)"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\SharpDevelop" \
                   UninstallString '"$INSTDIR\uninst.exe"'
  
  ; Delete old uninstaller first
  Delete $INSTDIR\uninst.exe
  WriteUninstaller $INSTDIR\uninst.exe

  ; now finally call our mini setup
  ExecWait "$INSTDIR\bin\tools\minisetup.exe"

SectionEnd

; Uninstall section
Section Uninstall
  ; Delete registry settings
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\SharpDevelop"
  DeleteRegKey HKLM SOFTWARE\SharpDevelop
  DeleteRegKey HKLM SYSTEM\CurrentControlSet\Services\EventLog\Application\SharpDevelop

  ; Remove start menu stuff
  Delete $SMPROGRAMS\SharpDevelop\*.lnk
  Delete $SMPROGRAMS\SharpDevelop\*.log
  RMDir $SMPROGRAMS\SharpDevelop

  ; Recursively remove program files
  RMDir /r $INSTDIR

  ;Display the Finish header
  !insertmacro MUI_UNFINISHHEADER
SectionEnd

