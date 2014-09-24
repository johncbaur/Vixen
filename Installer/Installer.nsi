; Script generated by the HM NIS Edit Script Wizard.

SetCompressor /FINAL /SOLID lzma
SetCompressorDictSize 64



; ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Macro: define a variable if a file exists ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

; See http://nsis.sourceforge.net/Check_if_a_file_exists_at_compile_time for documentation
!macro defineifexist _VAR_NAME _FILE_NAME
	!tempfile _TEMPFILE
	!ifdef NSIS_WIN32_MAKENSIS
		; Windows - cmd.exe
		!system 'if exist "${_FILE_NAME}" echo !define ${_VAR_NAME} > "${_TEMPFILE}"'
	!else
		; Posix - sh
		!system 'if [ -e "${_FILE_NAME}" ]; then echo "!define ${_VAR_NAME}" > "${_TEMPFILE}"; fi'
	!endif
	!include '${_TEMPFILE}'
	!delfile '${_TEMPFILE}'
	!undef _TEMPFILE
!macroend


; ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Macro: Get a binary file version into variables ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

!macro GetVersionLocal file basedef
!verbose push
!verbose 1
!tempfile _GetVersionLocal_nsi
!tempfile _GetVersionLocal_exe
!appendfile "${_GetVersionLocal_nsi}" 'Outfile "${_GetVersionLocal_exe}"$\nRequestexecutionlevel user$\n'
!appendfile "${_GetVersionLocal_nsi}" 'Section$\n!define D "$"$\n!define N "${D}\n"$\n'
!appendfile "${_GetVersionLocal_nsi}" 'GetDLLVersion "${file}" $2 $4$\n'
!appendfile "${_GetVersionLocal_nsi}" 'IntOp $1 $2 / 0x00010000$\nIntOp $2 $2 & 0x0000FFFF$\n'
!appendfile "${_GetVersionLocal_nsi}" 'IntOp $3 $4 / 0x00010000$\nIntOp $4 $4 & 0x0000FFFF$\n'
!appendfile "${_GetVersionLocal_nsi}" 'FileOpen $0 "${_GetVersionLocal_nsi}" w$\nStrCpy $9 "${N}"$\n'
!appendfile "${_GetVersionLocal_nsi}" 'FileWrite $0 "!define ${basedef}1 $1$9"$\nFileWrite $0 "!define ${basedef}2 $2$9"$\n'
!appendfile "${_GetVersionLocal_nsi}" 'FileWrite $0 "!define ${basedef}3 $3$9"$\nFileWrite $0 "!define ${basedef}4 $4$9"$\n'
!appendfile "${_GetVersionLocal_nsi}" 'FileClose $0$\nSectionend$\n'
!system '"${NSISDIR}\makensis" -NOCD -NOCONFIG "${_GetVersionLocal_nsi}"' = 0
!system '"${_GetVersionLocal_exe}" /S' = 0
!delfile "${_GetVersionLocal_exe}"
!undef _GetVersionLocal_exe
!include "${_GetVersionLocal_nsi}"
!delfile "${_GetVersionLocal_nsi}"
!undef _GetVersionLocal_nsi
!verbose pop
!macroend


; ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Support different archtecture builds -- 32 or 64 bit ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

!define BUILDARCH_DEFAULT 64

!ifndef BUILDARCH
	!define BUILDARCH ${BUILDARCH_DEFAULT}
!else
	!if ${BUILDARCH} != 32
		!if ${BUILDARCH} != 64
			!error "BUILDARCH isn't set to either 32 or 64 bit: ${BUILDARCH}"
		!endif
	!endif
!endif

!echo "Building installer for ${BUILDARCH}-bit architecture."



; ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Ensure we're always in the root of the source tree ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

; This can be pretty seriously improved -- it's only really designed to get out of the Installer/ directory.

!insertmacro defineifexist IN_SOLUTION_ROOT ".\Vixen.sln"

!ifndef IN_SOLUTION_ROOT
	!cd ..
!endif

!define INSTALLERDIR ".\Installer"


; ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Define the common variables for use in the rest of the script ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

!if ${BUILDARCH} == 32
	!define BUILD_DIR ".\Release"
	!define BITS 32
	!define BITS_READABLE "32-bit"
	!define PROG_FILES $PROGRAMFILES
!else
	!define BUILD_DIR ".\Release64"
	!define BITS 64
	!define BITS_READABLE "64-bit"
	!define PROG_FILES $PROGRAMFILES64
!endif



; Get the application binary version
!insertmacro GetVersionLocal "${BUILD_DIR}\VixenApplication.exe" AssemblyVersion_
VIProductVersion "${AssemblyVersion_1}.${AssemblyVersion_2}.${AssemblyVersion_3}.${AssemblyVersion_4}"
VIAddVersionKey "FileVersion" "${AssemblyVersion_1}.${AssemblyVersion_2}.${AssemblyVersion_3}.${AssemblyVersion_4}"

!if ${AssemblyVersion_1} == 0
	!define DEVBUILD
!endif

!define MAJORVERSION ${AssemblyVersion_1}
!define MINORVERSION ${AssemblyVersion_2}
!define BUILDNUMBER ${AssemblyVersion_3}

!define PRODUCT_NAME "Vixen"

!ifdef DEVBUILD
	!define PRODUCT_NAME_FULL "Vixen Development Build"
	Name "${PRODUCT_NAME} Development Build ${BUILDNUMBER} (${BITS_READABLE})"
	OutFile ".\${PRODUCT_NAME}-DevBuild-${BUILDNUMBER}-Setup-${BITS}bit.exe"
	InstallDir "${PROG_FILES}\Vixen Development Build"
!else
	!define PRODUCT_NAME_FULL "Vixen"
	Name "${PRODUCT_NAME} ${MAJORVERSION}.${MINORVERSION} (${BITS_READABLE})"
	OutFile ".\${PRODUCT_NAME}-${MAJORVERSION}.${MINORVERSION}-Setup-${BITS}bit.exe"
	InstallDir "${PROG_FILES}\Vixen"
!endif


; This will optionally get the install dir from the registry at the specified location; we don't want to use that for now,
; so will comment this out for the time being.
;InstallDirRegKey HKLM "${PRODUCT_DIR_REGKEY}" ""



; ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Rest of the installer: as it previously was (needs documenting) ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


; HM NIS Edit Wizard helper defines
!define PRODUCT_VERSION "${AssemblyVersion_1}.${AssemblyVersion_2}.${AssemblyVersion_3}"
!define PRODUCT_PUBLISHER "Vixen - Lighting Automation"
!define PRODUCT_WEB_SITE "http://www.vixenlights.com/"
!define PRODUCT_DIR_REGKEY "Software\Microsoft\Windows\CurrentVersion\App Paths\VixenApplication.exe"
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
!define PRODUCT_UNINST_ROOT_KEY "HKLM"
!define PRODUCT_STARTMENU_REGVAL "NSIS:StartMenuDir"










; for logging
!define LVM_GETITEMCOUNT 0x1004
!define LVM_GETITEMTEXT 0x102D


; MUI 1.67 compatible ------
;!include "MUI.nsh"

!include MUI2.nsh ;Modern interface
!include LogicLib.nsh ;nsDialogs

; MUI Settings
!define MUI_ABORTWARNING
;!define MUI_ICON "${NSISDIR}\Contrib\Graphics\Icons\orange-install.ico"
!define MUI_ICON "${INSTALLERDIR}\vixen.ico"
!define MUI_UNICON "${NSISDIR}\Contrib\Graphics\Icons\orange-uninstall.ico"
!define MUI_WELCOMEFINISHPAGE_BITMAP "${INSTALLERDIR}\vixen.bmp"



; Welcome page
!insertmacro MUI_PAGE_WELCOME
; License page
!insertmacro MUI_PAGE_LICENSE "${INSTALLERDIR}\license.txt"
; Release notes page
!define MUI_PAGE_HEADER_TEXT "Release notes"
!define MUI_PAGE_HEADER_SUBTEXT "Short summary of changes for each version of ${PRODUCT_NAME}."
!define MUI_LICENSEPAGE_TEXT_TOP "Press Page Down to see the rest of the release notes file."
!define MUI_LICENSEPAGE_TEXT_BOTTOM "When you have finished reading, click on Next to proceed."
!define MUI_LICENSEPAGE_BUTTON $(^NextBtn)
!insertmacro MUI_PAGE_LICENSE "${BUILD_DIR}\Release Notes.txt"

; Directory page
DirText "Setup will install ${PRODUCT_NAME_FULL} in the following folder. $\n$\nTo install in a different folder (such as a USB Drive), click Browse and select another folder. $\nWhen ready, click next to continue."
!insertmacro MUI_PAGE_DIRECTORY
; Start menu page
var ICONS_GROUP
!define MUI_STARTMENUPAGE_NODISABLE
!define MUI_STARTMENUPAGE_DEFAULTFOLDER "${PRODUCT_NAME_FULL}"
!define MUI_STARTMENUPAGE_REGISTRY_ROOT "${PRODUCT_UNINST_ROOT_KEY}"
!define MUI_STARTMENUPAGE_REGISTRY_KEY "${PRODUCT_UNINST_KEY}"
!define MUI_STARTMENUPAGE_REGISTRY_VALUENAME "${PRODUCT_STARTMENU_REGVAL}"
!insertmacro MUI_PAGE_STARTMENU Application $ICONS_GROUP
; Instfiles page
!insertmacro MUI_PAGE_INSTFILES
; Finish page
!define MUI_FINISHPAGE_RUN "$INSTDIR\VixenApplication.exe"
!define MUI_FINISHPAGE_SHOWREADME "$INSTDIR\Release Notes.txt"
!insertmacro MUI_PAGE_FINISH

; Uninstaller pages
!insertmacro MUI_UNPAGE_INSTFILES

; Language files
!insertmacro MUI_LANGUAGE "English"

; MUI end ------

ShowInstDetails show
ShowUnInstDetails show


RequestExecutionLevel admin ;Require admin rights on NT6+ (When UAC is turned on)
Var InstallDotNET

Function .onInit
	;Here we check to see if the user is an administrator. (Needed to see if we have .net)
	UserInfo::GetAccountType
	pop $0
	${If} $0 != "admin" ;Require admin rights on NT4+
		MessageBox mb_iconstop "Administrator rights required!"
		SetErrorLevel 740 ;ERROR_ELEVATION_REQUIRED
		Quit
	${EndIf}


	;Here we check for Client .NET 4.0 profile. To check for Full .NET 4.0, replace "Client" with "Full"
	ReadRegDWORD $0 HKLM 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full' Install
	${If} $0 == ''
		StrCpy $InstallDotNET "Yes"
		MessageBox MB_OK|MB_ICONINFORMATION "${PRODUCT_NAME} requires that the Microsoft .NET Framework 4.0 is installed. The Microsoft .NET Framework will be downloaded and installed automatically during installation of ${PRODUCT_NAME}."
		Return
	${EndIf}
	
FunctionEnd


Section "Application" SEC01

	; Get .NET if required
	${If} $InstallDotNET == "Yes"
		inetc::get /caption "Downloading Microsoft .NET Framework 4.0" /canceltext "Cancel" "http://download.microsoft.com/download/9/5/A/95A9616B-7A37-4AF6-BC36-D6EA96C8DAAE/dotNetFx40_Full_x86_x64.exe" "$TEMP\dotNetFx40_Full_x86_x64.exe" /end
		Pop $1
 
		${If} $1 != "OK"
			Delete "$TEMP\dotNetFx40_Full_x86_x64.exe"
			Abort "Installation cancelled."
		${EndIf}
		 
		ExecWait "$TEMP\dotNetFx40_Full_x86_x64.exe"
		Delete "$TEMP\dotNetFx40_Full_x86_x64.exe"
	${EndIf}

  ; only overwrite these if this installer has a newer version
  SetOverwrite ifnewer
  SetOutPath "$INSTDIR"
  File /r /x *.res /x *.obj /x *.pch /x *.pdb "${BUILD_DIR}\*.*"


  ; Shortcuts
  !insertmacro MUI_STARTMENU_WRITE_BEGIN Application
  ;CreateDirectory "$SMPROGRAMS\$ICONS_GROUP"
  ;CreateShortCut "$SMPROGRAMS\$ICONS_GROUP\Vixen ${MAJORVERSION}.lnk" "$INSTDIR\VixenApplication.exe"
  ;CreateShortCut "$DESKTOP\Vixen ${MAJORVERSION}.lnk" "$INSTDIR\VixenApplication.exe"
  ;CreateDirectory "$INSTDIR"
  ;CreateShortCut "$QUICKLAUNCH\Vixen ${MAJORVERSION}.lnk" "$INSTDIR\VixenApplication.exe"

  CreateDirectory "$SMPROGRAMS\$ICONS_GROUP"
  CreateShortCut "$SMPROGRAMS\$ICONS_GROUP\${PRODUCT_NAME_FULL}.lnk" "$INSTDIR\VixenApplication.exe"
  CreateShortCut "$SMPROGRAMS\$ICONS_GROUP\Uninstall.lnk" "$INSTDIR\uninst.exe"
  WriteIniStr "$INSTDIR\${PRODUCT_NAME_FULL}.url" "InternetShortcut" "URL" "${PRODUCT_WEB_SITE}"
  ;All users Icons
  CreateShortCut "$SMPROGRAMS\$ICONS_GROUP\${PRODUCT_NAME} Lights Website.lnk" "$INSTDIR\${PRODUCT_NAME}.url"

  ;SetShellVarContext all		; scope is "All Users"
  CreateShortCut "$DESKTOP\${PRODUCT_NAME_FULL}.lnk" "$INSTDIR\VixenApplication.exe"
  !insertmacro MUI_STARTMENU_WRITE_END


  
  Push $0 ; save

  Push "Marker"
  ; AccessControl::GrantOnFile "$INSTDIR" "(BU)" "GenericRead + GenericExecute + GenericWrite + Delete"
  AccessControl::GrantOnFile "$INSTDIR" "(S-1-5-32-545)" "FullAccess"
  Pop $0 ; get "Marker" 
  StrCmp $0 "Marker" Continue
  ;MessageBox MB_OK|MB_ICONSTOP "Setting access control for $INSTDIR: $0"
  Pop $0 ; pop "Marker"

  Continue:
  Pop $0 ; restore
  
SectionEnd


Section -Post
  WriteUninstaller "$INSTDIR\uninst.exe"
  WriteRegStr HKLM "${PRODUCT_DIR_REGKEY}" "" "$INSTDIR\VixenApplication.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "InstDir" "$INSTDIR"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayIcon" "$INSTDIR\VixenApplication.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEB_SITE}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER}"
  ; Write out the install log
  StrCpy $0 "$INSTDIR\install.log"
  Push $0
  Call DumpLog
SectionEnd


Function un.onUninstSuccess
  HideWindow
  MessageBox MB_ICONINFORMATION|MB_OK "$(^Name) was successfully removed from your computer."
FunctionEnd

Function un.onInit
  MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 "Are you sure you want to completely remove $(^Name) and all of its components?" IDYES +2
  Abort
FunctionEnd

Section Uninstall
  !insertmacro MUI_STARTMENU_GETFOLDER "Application" $ICONS_GROUP
  Delete /REBOOTOK "$INSTDIR\${PRODUCT_NAME}.url"
  Delete /REBOOTOK "$INSTDIR\uninst.exe"
  Delete /REBOOTOK "$INSTDIR\VixenApplication.exe.config"
  Delete /REBOOTOK "$INSTDIR\VixenApplication.exe"
  Delete /REBOOTOK "$INSTDIR\Vixen.dll"
  Delete /REBOOTOK "$INSTDIR\SciLexer64.dll"
  Delete /REBOOTOK "$INSTDIR\SciLexer.dll"
  Delete /REBOOTOK "$INSTDIR\Release Notes.txt"
  Delete /REBOOTOK "$INSTDIR\Modules\Timing\Generic.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\SequenceType\Vixen2x.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\SequenceType\Timed.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\SequenceType\Script.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\SequenceType\BaseSequence.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\SequenceFilter\FadeOut.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\Script\VB.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\Script\CSharp.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\Property\Position.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\Preview\DisplayPreview.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\OutputFilter\DimmingCurve.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\OutputFilter\ColorBreakdown.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\Media\Audio.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\EffectEditor\ValueTypes.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\EffectEditor\TwinkleEffectEditor.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\EffectEditor\SpinEffectEditor.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\EffectEditor\PercentageTypeEditor.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\EffectEditor\LevelTypeEditor.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\EffectEditor\IntUpDownEditor.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\EffectEditor\FilePathTypeEditor.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\EffectEditor\CurveTypeEditor.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\EffectEditor\ColorTypeEditor.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\EffectEditor\ColorGradientTypeEditor.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\EffectEditor\ChaseEffectEditor.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\Effect\Twinkle.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\Effect\Spin.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\Effect\SetPosition.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\Effect\SetLevel.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\Effect\Pulse.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\Effect\Chase.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\Effect\Candle.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\Editor\TimedSequenceEditor.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\Editor\ScriptEditor.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\Editor\ScintillaNET.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\Controller\Renard.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\Controller\PSC.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\Controller\OpenDMX.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\Controller\Olsen595.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\Controller\Hill320.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\Controller\GenericSerial.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\Controller\FGDimmer.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\Controller\E131.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\Controller\DummyLighting.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\Controller\DmxUsbPro.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\App\StateMach.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\App\Scheduler.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\App\InstrumentationPanel.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\App\Curves.dll"
  Delete /REBOOTOK "$INSTDIR\Modules\App\ColorGradients.dll"
  Delete /REBOOTOK "$INSTDIR\inpoutx64.dll"
  Delete /REBOOTOK "$INSTDIR\inpout32.dll"
  Delete /REBOOTOK "$INSTDIR\Common\ValueTypes.dll"
  Delete /REBOOTOK "$INSTDIR\Common\ScriptSequence.dll"
  Delete /REBOOTOK "$INSTDIR\Common\fmodex64.dll"
  Delete /REBOOTOK "$INSTDIR\Common\fmodex.dll"
  Delete /REBOOTOK "$INSTDIR\Common\FMOD.dll"
  Delete /REBOOTOK "$INSTDIR\Common\Dataweb.NShape.WinFormsUI.xml"
  Delete /REBOOTOK "$INSTDIR\Common\Dataweb.NShape.WinFormsUI.dll"
  Delete /REBOOTOK "$INSTDIR\Common\Dataweb.NShape.GeneralShapes.dll"
  Delete /REBOOTOK "$INSTDIR\Common\Dataweb.NShape.dll"
  Delete /REBOOTOK "$INSTDIR\Common\Controls.dll"
  Delete /REBOOTOK "$INSTDIR\Common\BaseSequence.dll"

  Delete /REBOOTOK "$SMPROGRAMS\$ICONS_GROUP\Uninstall.lnk"
  Delete /REBOOTOK "$SMPROGRAMS\$ICONS_GROUP\Website.lnk"
  Delete /REBOOTOK "$DESKTOP\${PRODUCT_NAME_FULL}.lnk"
  Delete /REBOOTOK "$SMPROGRAMS\$ICONS_GROUP\Vixen.lnk"

  RMDir /r /REBOOTOK "$SMPROGRAMS\$ICONS_GROUP"
  ; Here we force removing the directories... Clean it up!
  RMDir /r /REBOOTOK "$INSTDIR\Modules"
  RMDir /r /REBOOTOK "$INSTDIR\Common"
  RMDir /r /REBOOTOK "$INSTDIR"

  DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}"
  DeleteRegKey HKLM "${PRODUCT_DIR_REGKEY}"
  
  IfRebootFlag r_boot nr_boot
  
  r_boot:
    MessageBox MB_OKCANCEL "You must now reboot your system to complete the uninstall." IDCANCEL end
    Reboot
  goto end
  
  nr_boot:
  end:
  
  SetAutoClose true
SectionEnd




Function DumpLog
  Exch $5
  Push $0
  Push $1
  Push $2
  Push $3
  Push $4
  Push $6

  FindWindow $0 "#32770" "" $HWNDPARENT
  GetDlgItem $0 $0 1016
  StrCmp $0 0 exit
  FileOpen $5 $5 "w"
  StrCmp $5 "" exit
    SendMessage $0 ${LVM_GETITEMCOUNT} 0 0 $6
    System::Alloc ${NSIS_MAX_STRLEN}
    Pop $3
    StrCpy $2 0
    System::Call "*(i, i, i, i, i, i, i, i, i) i \
      (0, 0, 0, 0, 0, r3, ${NSIS_MAX_STRLEN}) .r1"
    loop: StrCmp $2 $6 done
      System::Call "User32::SendMessageA(i, i, i, i) i \
        ($0, ${LVM_GETITEMTEXT}, $2, r1)"
      System::Call "*$3(&t${NSIS_MAX_STRLEN} .r4)"
      FileWrite $5 "$4$\r$\n"
      IntOp $2 $2 + 1
      Goto loop
    done:
      FileClose $5
      System::Free $1
      System::Free $3
  exit:
    Pop $6
    Pop $4
    Pop $3
    Pop $2
    Pop $1
    Pop $0
    Exch $5
FunctionEnd




;Example Code - .NET Config Files
;
;I'm using this plugin to adjust .NET config files. It works wonderfully. Here's what it looks like:
;!macro AdjustConfigValue ConfigFile  Key Value

;   DetailPrint "Config: adding '${Key}'='${Value}' to ${ConfigFile}"

;   nsisXML::create
;   nsisXML::load ${ConfigFile}

;   nsisXML::select "/configuration/appSettings/add[@key='${Key}']"
;   nsisXML::setAttribute "value" ${Value}

;   nsisXML::save ${ConfigFile}

;!macroend

;!insertmacro AdjustConfigValue "$INSTDIR\MyApp.exe.config" "ServiceURL" "http://127.0.0.1"
