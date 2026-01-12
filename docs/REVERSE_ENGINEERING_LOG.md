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

## Deep Dive: SnxHidLib.dll Internal Protocol (2026-01-13)

### Data Structures

| Address | Size | Purpose |
|---------|------|---------|
| `0x1005d0b8` | DWORD | Current device index |
| `0x1005d0bc` | WORD | Target VendorID |
| `0x1005d0c0` | WORD | Target ProductID |
| `0x1005d0d0` | 64+ bytes | Feature report buffer |
| `0x1004bd40` | 0x270 * N | Device struct array (624 bytes each) |
| `0x1004bf48` | WORD | FeatureReportByteLength (offset +0x208 into device struct) |

### Core Functions

#### `fcn.10002bc0` - SetFeature Wrapper (351 bytes)
1. Reads device index from `0x1005d0b8`
2. Computes device struct address: `0x1004bd40 + (index * 0x270)`
3. Opens device via `CreateFileA` with `GENERIC_READ | GENERIC_WRITE` (0xC0000000)
4. Zeros first byte of buffer at `0x1005d0d0`
5. Copies user data to `0x1005d0d1` (length = FeatureReportByteLength - 1)
6. Calls `HidD_SetFeature(handle, buffer, length)`
7. Closes handle

#### `fcn.10002d20` - GetFeature Wrapper (335 bytes)
- Mirror of SetFeature but calls `HidD_GetFeature`
- Copies response data from `0x1005d0d1` back to user buffer

#### `fcn.10003370` - Delay/Timing Function (114 bytes)
- Called before read/write with argument `5` (ms delay?)
- Uses `QueryPerformanceCounter` / `QueryPerformanceFrequency`
- Implements a spin-wait loop for precise timing (~1000 ticks = 0x3e8)

#### `SnxISP_WriteData` (65 bytes)
```
push 5
call fcn.10003370          ; 5ms delay
mov eax, [arg_ch]          ; data length
mov ecx, [arg_8h]          ; data pointer
movzx edx, word [0x1004b58c]  ; some config word
push eax
movzx eax, word [0x1004b694]  ; another config word
push ecx
push edx
push eax
call fcn.10002bc0          ; SetFeature
```

### Report Structure
- **Report ID**: Byte 0 is ALWAYS zeroed by the DLL before copying user data
- **User Data**: Starts at byte 1
- **Max Length**: `FeatureReportByteLength - 1`
- The DLL handles the report ID internally - callers just provide raw payload

### Device Enumeration (fcn.10005190)
- Tries 3 different GUID sets for HID device discovery
- Stores GUIDs at `0x1004bd1c` - `0x1004bd28`
- On success, sends Windows message 0x4402 (device found)
- On failure, sends Windows message 0x4403 (device not found)

### Key Insight: No Hardcoded Init Sequence
The DLL is a **generic SONiX ISP library** - it does NOT contain Filco-specific initialization commands. The actual protocol commands are in `FilcoAssistUI.exe` (the JUCE/C++ app), which:
1. Calls `SnxISP_SetVendorID(0x2efd)` and `SnxISP_SetProductID(0x1972)`
2. Calls `SnxISP_OpenISPDevice()`
3. Sends Filco-specific commands via `SnxISP_WriteData(data, len)`

### Next Steps for Protocol Discovery
1. **Dynamic Analysis (Recommended)**: Capture USB traffic using Wireshark + USBPcap on Windows while FILCO Assist initializes the keyboard
2. **UI Binary Analysis**: Reverse `FilcoAssistUI.exe` C++ code to find the `CostarKBHID::SetKeyDefinitionTable` implementation
3. **Runtime Hooking**: Hook `SnxISP_WriteData` to log all commands sent to the keyboard
