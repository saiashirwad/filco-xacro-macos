# Hardware Programming Guide: M1-M10 Keys

The Filco Xacro M10SP supports hardware-level macro recording. This is the most reliable way to use the macro keys on macOS without a proprietary driver.

## Step-by-Step Programming

For each macro key (M1-M10):

1.  **Enter Macro Mode**: Press `Fn + Ctrl`. The LED will flash **RED**.
2.  **Select Target Key**: Press the macro key you wish to program (e.g., **M1**). The LED will flash **YELLOW**.
3.  **Record Action**:
    -   Type the key sequence or single key you want the macro to perform.
    -   *Recommended for macOS*: Map them to **F13 through F22** (see table below).
4.  **Save Macro**: Press `Fn + Alt`. The LED will flash **GREEN**.
5.  **Exit Macro Mode**: Press `Fn + Ctrl`.

## Recommended macOS Mapping

Since macOS does not use F13-F24 by default, mapping the macro keys to these codes allows you to use **Karabiner-Elements** to assign them to any macOS shortcut (e.g., Mission Control, Spotlight, App Switcher).

| Macro Key | Recommended Key | HID Code |
|-----------|-----------------|----------|
| M1 | F13 | `0x68` |
| M2 | F14 | `0x69` |
| M3 | F15 | `0x6A` |
| M4 | F16 | `0x6B` |
| M5 | F17 | `0x6C` |
| M6 | F18 | `0x6D` |
| M7 | F19 | `0x6E` |
| M8 | F20 | `0x6F` |
| M9 | F21 | `0x70` |
| M10 | F22 | `0x71` |

## DIP Switch Reference

| SW | Function | Recommendation |
|----|----------|----------------|
| 1 | Swap ESC and ` | User Preference |
| 2 | Swap Caps Lock and Left Ctrl | User Preference |
| 3 | Windows/Mac mode | **ON** (Mac Mode) |
| 4 | Layout Selection | User Preference |

---
*Note: If your keyboard does not have physical F13-F22 keys to press during recording, you can use the macOS **Keyboard Viewer** or connect a second standard keyboard temporarily to send the F-key signals during step 3.*
