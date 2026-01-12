# Filco Xacro M10SP Reverse Engineering Findings

## Keyboard Identification
- **Vendor ID**: 0x2efd (SONiX)
- **Product ID**: 0x1972
- **Manufacturer**: SONiX
- **MCU**: SONiX SN8 series (based on Cortex-M0)

## HID Interfaces Exposed
The keyboard exposes 8 HID interfaces:

| Usage Page | Usage | Purpose |
|------------|-------|---------|
| 0x0001 | 0x06 | Standard Keyboard |
| 0x0001 | 0x06 | Standard Keyboard (2nd) |
| 0x0001 | 0x02 | Mouse/Pointer |
| 0x000C | 0x01 | Consumer Controls (Media Keys) |
| 0xFFA0 | 0x01 | **Vendor-Specific (Macro Keys?)** |
| 0xFFFF | 0x01 | **Vendor-Specific (Config?)** |
| 0x0001 | 0x06 | Standard Keyboard (3rd) |
| 0x0001 | 0x06 | Standard Keyboard (4th) |

## FILCO Assist Analysis
- Built with JUCE framework (C++)
- Uses SnxHidLib.DLL for SONiX HID communication
- Internal namespace: `i3d::costar::CostarKBHID`
- Source path: `D:\works\Costar\costar-kb.git\`

### Key Functions Found
- `CostarKBHID::ResetToDefault`
- `CostarKBHID::SendFinishedCommand`
- `CostarKBHID::SetKeyDefinitionTable`
- `CostarKBHID::GetGameModeDisabledKeys`
- `PerformEntireProfileToKeyboard`

### HID Protocol (from SnxHidLib.dll)
- Uses `HidD_SetFeature` / `HidD_GetFeature` for control channel
- Uses `ReadFile` / `WriteFile` for data transfer
- Feature report buffer at fixed address with `FeatureReportByteLength` size
- Report IDs and command structure are proprietary

## macOS Limitations
1. **System claims HID devices**: Most interfaces return "open failed" because macOS kernel claims them
2. **Vendor interfaces accessible but silent**: 0xFFA0 and 0xFFFF can be opened via hidapi but:
   - `send_feature_report` returns -1 (error)
   - `write` returns -1 (error)  
   - `read` returns nothing (no data)
3. **No initialization = no events**: Without Windows driver sending init commands, macro keys don't send events

## Root Cause
The M1-M10 macro keys require **initialization from the Windows driver** before they send HID events. Without this:
- Keys are electrically connected to the MCU
- MCU firmware recognizes key presses
- But the HID report generation for macro keys is disabled by default
- Windows FILCO Assist sends specific feature reports to enable macro key reporting

## Possible Solutions

### Option 1: Hardware Macro Mode (Recommended - Works Now)
The keyboard has **built-in hardware macro recording** that works without any software:

1. Press `Fn + Ctrl` to enter macro recording mode
2. Press a macro key (M1-M10) to select it
3. Type the key sequence you want
4. Press `Fn + Ctrl` again to save

Hardware macros are stored in keyboard EEPROM and work on any OS. They send standard keyboard scancodes.

**Limitation**: Can only record key sequences, not arbitrary actions.

### Option 2: USB Protocol Capture (Requires Windows VM)
1. Run Windows in Parallels/VMware/VirtualBox
2. Pass USB keyboard to Windows VM
3. Install FILCO Assist
4. Use Wireshark with USBPcap to capture initialization sequence
5. Replay captured commands on macOS

**Effort**: High (requires Windows VM, USB passthrough, Wireshark analysis)

### Option 3: Firmware Modification (Advanced)
Based on SonixQMK project:
1. Dump existing firmware via SWD debug port
2. Port QMK to SONiX MCU
3. Flash custom firmware with raw HID support

**Effort**: Very high (hardware hacking, MCU programming)
**Risk**: Could brick the keyboard

### Option 4: Use Different Keys
Remap existing keyboard keys (that work on macOS) to act as macro triggers using Karabiner-Elements.

## Recommendation

**Short term**: Use Option 1 (hardware macro mode). It already works:
- Macros stored in keyboard work on macOS
- No software needed
- Press Fn+Ctrl → select macro key → type sequence → Fn+Ctrl to save

**If custom actions needed**: Use Option 2 to capture the protocol:
- Set up Windows VM with USB passthrough
- Capture HID traffic when FILCO Assist initializes the keyboard
- Implement macOS daemon to replay initialization
- Then macro keys should send events that hidapi can read

## Files Created
- `/Users/texoport/code/filco/filco_hid_probe.py` - HID interface scanner
- `/Users/texoport/code/filco/filco_hid_protocol.py` - Protocol tester
- `/Users/texoport/code/filco/filco_daemon.py` - Daemon framework
- `/Users/texoport/code/filco/reverse/` - Decompiled FILCO Assist files
