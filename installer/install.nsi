# Use Modern UI -------------------------------------------------------------------------------------------------------
!include "MUI2.nsh"
!include NTProfiles.nsh

# Global installer configuration --------------------------------------------------------------------------------------
!define APPNAME "SeaShell"
!define COMPANYNAME "carlubian"
!define HELPURL "https://www.github.com/carlubian/SeaShell"
!define INSTALLSIZE 100
!define VERSIONMAJOR 0
!define VERSIONMINOR 5
!define VERSIONPATCH 0
!define VERSIONBUILD 030720

RequestExecutionLevel admin
InstallDir "$PROGRAMFILES\${APPNAME}"
!define LIBDIR "$USERPROFILE\.SeaShell"

# Visual look ---------------------------------------------------------------------------------------------------------
Name "${APPNAME}"
!define MUI_ICON "icon-SeaShell.ico"
!define MUI_BGCOLOR "283038"
!define MUI_TEXTCOLOR "F0F5FF"
OutFile "SeaShell-0.5.0.exe"

# Page definition -----------------------------------------------------------------------------------------------------
!insertmacro MUI_PAGE_WELCOME
!define MUI_LICENSEPAGE_BUTTON "Oh, God!"
!insertmacro MUI_PAGE_LICENSE "License.txt"
!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_DIRECTORY
Var StartMenuFolder
!insertmacro MUI_PAGE_STARTMENU Application $StartMenuFolder
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_UNPAGE_WELCOME
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES

# Languages -----------------------------------------------------------------------------------------------------------
!define MUI_LANGDLL_ALLLANGUAGES
!insertmacro MUI_LANGUAGE "English"
!insertmacro MUI_LANGUAGE "Spanish"
!insertmacro MUI_RESERVEFILE_LANGDLL

# Show language selection ---------------------------------------------------------------------------------------------
Function .onInit

    !insertmacro MUI_LANGDLL_DISPLAY

FunctionEnd

# Core components -----------------------------------------------------------------------------------------------------
Section "SeaShell" SectionCore

    SetOutPath $INSTDIR
    File "Core\ConfigAdapter.dll"
    File "Core\ConfigAdapter.Ini.dll"
    File "Core\ConfigAdapter.Xml.dll"
    File "Core\DotNet.Misc.Extensions.dll"
    File "Core\DotNetZip.dll"
    File "Core\IniFileParserDotNetCore.dll"
    File "Core\Pastel.dll"
    File "Core\SeaShell.Core.dll"
    File "Core\SeaShell.dll"
    File "Core\SeaShell.exe"
    File "Core\SeaShell.runtimeconfig.json"
    File "Core\Sprache.dll"
    File "Core\System.Security.Permissions.dll"

    WriteUninstaller "$INSTDIR\Uninstall.exe"

    # Registry information for add/remove programs
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "DisplayName" "${APPNAME}"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "UninstallString" "$\"$INSTDIR\uninstall.exe$\""
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "QuietUninstallString" "$\"$INSTDIR\uninstall.exe$\" /S"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "InstallLocation" "$\"$INSTDIR$\""
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "DisplayIcon" "$\"$INSTDIR\logo.ico$\""
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "Publisher" "$\"${COMPANYNAME}$\""
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "HelpLink" "$\"${HELPURL}$\""
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "DisplayVersion" "$\"${VERSIONMAJOR}.${VERSIONMINOR}.${VERSIONPATCH}.${VERSIONBUILD}$\""
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "VersionMajor" ${VERSIONMAJOR}
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "VersionMinor" ${VERSIONMINOR}
	
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "NoModify" 1
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "NoRepair" 1
	
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "EstimatedSize" ${INSTALLSIZE}

    # Start menu
    !insertmacro MUI_STARTMENU_WRITE_BEGIN Application
    
        CreateDirectory "$SMPROGRAMS\$StartMenuFolder"
        CreateShortCut "$SMPROGRAMS\$StartMenuFolder\${APPNAME}.lnk" "$INSTDIR\Core.txt" "" "$INSTDIR\logo.ico"
        CreateShortcut "$SMPROGRAMS\$StartMenuFolder\Uninstall.lnk" "$INSTDIR\Uninstall.exe"
  
    !insertmacro MUI_STARTMENU_WRITE_END

SectionEnd

# Otter section -------------------------------------------------------------------------------------------------------
Function InstallOtterOnProfile 
    ## Get the profile path from the stack
        Pop $0
 
    # Install Otter for current user profile
        SetOutPath $0\Libraries\SeaShell.Otter
        File "Otter\Manifest.ini"
        SetOutPath $0\Libraries\SeaShell.Otter\Assemblies
        File "Otter\DotNetZip.dll"
        File "Otter\SeaShell.Otter.dll"
 
    ## Continue Enumeration
        Push ""
        Return
 
    ## Stop Enumeration
        Stop:
        Push "~" # Any value other than an empty string will abort the enumeration
FunctionEnd

Section "Otter" SectionOtter

    !define NTProfilePaths::IgnoreSystem
    ${EnumProfilePaths} InstallOtterOnProfile

SectionEnd

# Unistall ------------------------------------------------------------------------------------------------------------
Function Un.installOtterOnProfile 
    ## Get the profile path from the stack
        Pop $0
 
    # Uninstall Otter for current user profile
        Delete $0\Libraries\SeaShell.Otter\Manifest.ini
        SetOutPath $0\Libraries\SeaShell.Otter\Assemblies
        Delete $0\Libraries\SeaShell.Otter\Assemblies\DotNetZip.dll
        Delete $0\Libraries\SeaShell.Otter\Assemblies\SeaShell.Otter.dll
 
    ## Continue Enumeration
        Push ""
        Return
 
    ## Stop Enumeration
        Stop:
        Push "~" # Any value other than an empty string will abort the enumeration
FunctionEnd

Section "Uninstall"

    !insertmacro MUI_STARTMENU_GETFOLDER Application $StartMenuFolder
 
	Delete "$SMPROGRAMS\$StartMenuFolder\${APPNAME}.lnk"
    Delete "$SMPROGRAMS\$StartMenuFolder\Uninstall.lnk"
	RmDir "$SMPROGRAMS\$StartMenuFolder"
 
	Delete $INSTDIR\ConfigAdapter.dll
    Delete $INSTDIR\ConfigAdapter.Ini.dll
    Delete $INSTDIR\ConfigAdapter.Xml.dll
    Delete $INSTDIR\DotNet.Misc.Extensions.dll
    Delete $INSTDIR\DotNetZip.dll
    Delete $INSTDIR\IniFileParserDotNetCore.dll
    Delete $INSTDIR\Pastel.dll
    Delete $INSTDIR\SeaShell.Core.dll
    Delete $INSTDIR\SeaShell.dll
    Delete $INSTDIR\SeaShell.exe
    Delete $INSTDIR\SeaShell.runtimeconfig.json
    Delete $INSTDIR\Sprache.dll
    Delete $INSTDIR\System.Security.Permissions.dll
	Delete $INSTDIR\icon-SeaShell.ico
	Delete $INSTDIR\Uninstall.exe

    !define NTProfilePaths::IgnoreSystem
    ${EnumProfilePaths} Un.installOtterOnProfile
 
	RmDir $INSTDIR
    RMDir ${LIBDIR}
 
	DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}"

SectionEnd

# Component descriptions ----------------------------------------------------------------------------------------------
LangString DESC_SectionCore $\{LANG_ENGLISH} "Required components for SeaShell."
LangString DESC_SectionCore $\{LANG_SPANISH} "Componentes necesarios para SeaShell."
LangString DESC_SectionOtter $\{LANG_ENGLISH} "Otter is the package manager for SeaShell. Without it, you will be unable to install or remove SeaShell libraries."
LangString DESC_SectionOtter $\{LANG_SPANISH} "Otter es el administrador de paquetes de SeaShell. Si no se instala, no podrás añadir o eliminar librerías de SeaShell."

!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
    !insertmacro MUI_DESCRIPTION_TEXT ${SectionCore} $(DESC_SectionCore)
    !insertmacro MUI_DESCRIPTION_TEXT ${SectionOtter} $(DESC_SectionOtter)
!insertmacro MUI_FUNCTION_DESCRIPTION_END
