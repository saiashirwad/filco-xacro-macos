# Reverse Engineering Log: FILCO Assist

## Binaries Extracted
The official installer `FILCOAssistSetup_2.0.97.exe` contains an embedded MSI, which in turn contains the application binaries.

| Binary | Type | Role |
|--------|------|------|
| `FilcoAssistUI.exe` | .NET / C++ (JUCE) | Main User Interface |
| `SnxHidLib.dll` | Native x86 DLL | SONiX HID Communication Layer |
| `FilcoAssistTray.exe` | .NET | System Tray Utility |

## Key Symbols Found (SnxHidLib.dll)
The library exports several functions prefixed with `SnxISP_`, indicating a focus on In-System Programming:
-   `SnxISP_OpenISPDevice`
-   `SnxISP_WriteData` / `SnxISP_ReadData`
-   `SnxISP_SetVendorID` / `SnxISP_SetProductID`
-   `SnxISP_GetSN8CheckSum`

## Protocol Analysis (FilcoAssistUI.exe)
Static analysis of the UI binary reveals the use of the `i3d::costar` namespace.

### Key Methods
-   `i3d::costar::CostarKBHID::SetKeyDefinitionTable`: Likely sends the mapping of M1-M10 to specific actions.
-   `i3d::costar::CostarKBHID::SendFinishedCommand`: Likely a "commit" or "end transaction" signal.
-   `i3d::costar::CostarKBHID::GetGameModeDisabledKeys`: Manages the Win-lock and other gaming features.

### Communication Pattern
The software interacts with the keyboard using **Feature Reports**. 
-   Buffer size: Determined by `FeatureReportByteLength`.
-   HID API calls: `HidD_SetFeature` and `HidD_GetFeature`.

## Extraction Commands Used
1.  **Extract MSI**: `python3 extract_resources.py` (Searches for OLE magic `D0 CF 11 E0`).
2.  **Extract Files from MSI**: `7z x embedded_0.msi`.
3.  **Decompile .NET**: `ilspycmd FilcoAssistUI.exe`.
4.  **Symbol Analysis**: `r2 SnxHidLib.dll` -> `aaa` -> `ii` (imports) / `afl` (functions).

## Known Strings
-   `D:\works\Costar\costar-kb.git\src\Vendor\costar\costar_kb_hid.cpp`: Indicates the OEM relationship with Costar.
-   `hid#vid_%04x&pid_%04x`: Standard Windows HID path string template.
