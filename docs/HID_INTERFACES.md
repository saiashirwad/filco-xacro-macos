# HID Interface Specification: Filco Xacro M10SP

The Filco Xacro M10SP exposes 8 distinct HID interfaces to the host system.

## Interface Table

| Index | Usage Page | Usage | Interface # | macOS Service | Purpose (Estimated) |
|-------|------------|-------|-------------|---------------|---------------------|
| 1 | `0x0001` | `0x02` | 1 | Mouse | Mouse/Pointer emulation |
| 2 | `0x0001` | `0x06` | 0 | Keyboard | Primary Keyboard Input |
| 3 | `0x000C` | `0x01` | 2 | Consumer | Media Keys (Vol+, Vol-, etc.) |
| 4 | `0xFFA0` | `0x01` | 3 | Vendor | **Macro Key Reporting** |
| 5 | `0xFFFF` | `0x01` | 4 | Vendor | **Configuration/ISP** |
| 6 | `0x0001` | `0x06` | 5 | Keyboard | Secondary Keyboard |
| 7 | `0x0001` | `0x06` | 6 | Keyboard | Tertiary Keyboard |
| 8 | `0x0001` | `0x06` | 7 | Keyboard | Quaternary Keyboard |

## Usage Details

### Vendor-Specific Interfaces (0xFFA0, 0xFFFF)
-   **0xFFA0**: Likely where the "Raw" macro key events are sent when the software is active.
-   **0xFFFF**: Likely used for ISP (In-System Programming) and firmware updates, managed by `SnxHidLib.dll`.

### macOS Accessibility
-   Interfaces with Usage Page `0x0001` and `0x000C` are usually locked by the macOS kernel.
-   Interfaces `0xFFA0` and `0xFFFF` can be opened by user-space applications (e.g., Python `hidapi`) without sudo, but they do not produce data without an initialization command.

## Probing Script
The script `filco_hid_probe.py` in the root directory can be used to verify these interfaces on any connected machine.
