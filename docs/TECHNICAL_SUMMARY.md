# Technical Summary: Filco Xacro M10SP on macOS

## Overview
The Filco Xacro M10SP is a split mechanical keyboard using a SONiX MCU. It features 10 macro keys (M1-M10) that do not send standard HID events by default on macOS. This project involved reverse engineering the official Windows software (FILCO Assist) to understand how it communicates with the device and exploring ways to enable these keys on macOS.

## Device Identification
- **Manufacturer**: SONiX
- **Product Name**: Gaming KeyBoard (Xacro M10SP)
- **Vendor ID (VID)**: `0x2efd`
- **Product ID (PID)**: `0x1972`
- **MCU**: SONiX SN8 series (Cortex-M0 based)

## Key Findings
1.  **Macro Key Behavior**: The M1-M10 keys do not generate standard keyboard scancodes until initialized or programmed.
2.  **HID Topology**: The keyboard exposes 8 HID interfaces. Most are standard (Keyboard, Mouse, Consumer Control), but two are vendor-specific (`0xFFA0` and `0xFFFF`).
3.  **Software Reverse Engineering**:
    -   Official software is `.NET` based but uses a native Win32 DLL (`SnxHidLib.dll`) for HID communication.
    -   The software uses **Feature Reports** to send commands to the keyboard.
    -   The internal codebase refers to the project as "Costar KB" (`i3d::costar::CostarKBHID`).
4.  **macOS Blocker**: Standard macOS HID APIs (`IOHIDManager`) claim the standard keyboard interfaces exclusively, preventing user-space scripts from reading them. The vendor-specific interfaces are accessible but remain silent unless a specific (and currently unknown) initialization sequence is sent.

## Work Performed
-   **Extraction**: Decompiled `FILCOAssistSetup_2.0.97.exe` using `7z`, `cabextract`, and custom Python scripts to extract the MSI and its embedded binaries.
-   **Analysis**: Analyzed `SnxHidLib.dll` and `FilcoAssistUI.exe` using `strings`, `radare2`, and `ilspycmd`.
-   **Probing**: Developed Python scripts using `hidapi` to enumerate and probe all 8 interfaces on macOS.
-   **Documentation**: Created manual programming guides and technical specification logs.

## Current Recommendation
Use the **Hardware Macro Mode** (Fn + Ctrl) to program the keys to send standard F13-F24 scancodes, then use **Karabiner-Elements** to remap them to desired macOS actions. This bypasses the need for the proprietary SONiX initialization protocol.
