# Filco Xacro M10SP - macOS Macro Key Configuration

## The Problem
The M1-M10 macro keys don't send any data by default. macOS cannot see them until they're programmed.

## The Solution
Program each macro key to send F13-F24 (unused by macOS), then use Karabiner-Elements to map those to any action.

---

## Part 1: Hardware Macro Programming

### Key Assignments
| Macro Key | Program To | HID Code |
|-----------|------------|----------|
| M1 | F13 | 0x68 |
| M2 | F14 | 0x69 |
| M3 | F15 | 0x6A |
| M4 | F16 | 0x6B |
| M5 | F17 | 0x6C |
| M6 | F18 | 0x6D |
| M7 | F19 | 0x6E |
| M8 | F20 | 0x6F |
| M9 | F21 | 0x70 |
| M10 | F22 | 0x71 |

### Programming Steps (Hardware Mode)

**For EACH macro key (M1-M10), repeat this process:**

1. **Enter Macro Mode**
   - Press `Fn + Ctrl`
   - Red LED will flash

2. **Select the Macro Key**
   - Press the macro key you want to program (e.g., M1)
   - Yellow LED will flash

3. **Record the Keypress**
   - Press the F-key you want (F13 for M1, F14 for M2, etc.)
   - Note: You need a full-size keyboard or use the method below

4. **Save the Macro**
   - Press `Fn + Alt`
   - Green LED will flash

5. **Exit Macro Mode**
   - Press `Fn + Ctrl`

### Alternative: Using Keyboard Viewer for F13-F24

Since your keyboard doesn't have F13-F24 physically:

1. Open **System Settings > Keyboard > Input Sources**
2. Enable **Show Input menu in menu bar**
3. Click the Input menu > **Show Keyboard Viewer**
4. When in macro recording mode, use another keyboard or input source

**OR use this USB keyboard trick:**
- Connect any USB keyboard temporarily
- Use it to press F13-F24 during macro recording

---

## Part 2: Karabiner-Elements Configuration

After programming, create `~/.config/karabiner/assets/complex_modifications/filco-macros.json`:

```json
{
  "title": "Filco Xacro M10SP Macro Keys",
  "rules": [
    {
      "description": "M1 (F13) - Mission Control",
      "manipulators": [{
        "type": "basic",
        "from": { "key_code": "f13" },
        "to": [{ "key_code": "mission_control" }]
      }]
    },
    {
      "description": "M2 (F14) - Application Windows",
      "manipulators": [{
        "type": "basic",
        "from": { "key_code": "f14" },
        "to": [{ "key_code": "launchpad" }]
      }]
    },
    {
      "description": "M3 (F15) - Screenshot",
      "manipulators": [{
        "type": "basic",
        "from": { "key_code": "f15" },
        "to": [{ "key_code": "4", "modifiers": ["command", "shift"] }]
      }]
    },
    {
      "description": "M4 (F16) - Spotlight",
      "manipulators": [{
        "type": "basic",
        "from": { "key_code": "f16" },
        "to": [{ "key_code": "spacebar", "modifiers": ["command"] }]
      }]
    },
    {
      "description": "M5 (F17) - Toggle Do Not Disturb",
      "manipulators": [{
        "type": "basic",
        "from": { "key_code": "f17" },
        "to": [{ "shell_command": "shortcuts run 'Toggle Focus'" }]
      }]
    },
    {
      "description": "M6 (F18) - Lock Screen",
      "manipulators": [{
        "type": "basic",
        "from": { "key_code": "f18" },
        "to": [{ "key_code": "q", "modifiers": ["command", "control"] }]
      }]
    },
    {
      "description": "M7 (F19) - Play/Pause",
      "manipulators": [{
        "type": "basic",
        "from": { "key_code": "f19" },
        "to": [{ "key_code": "play_or_pause" }]
      }]
    },
    {
      "description": "M8 (F20) - Previous Track",
      "manipulators": [{
        "type": "basic",
        "from": { "key_code": "f20" },
        "to": [{ "key_code": "rewind" }]
      }]
    },
    {
      "description": "M9 (F21) - Next Track",
      "manipulators": [{
        "type": "basic",
        "from": { "key_code": "f21" },
        "to": [{ "key_code": "fastforward" }]
      }]
    },
    {
      "description": "M10 (F22) - Volume Mute",
      "manipulators": [{
        "type": "basic",
        "from": { "key_code": "f22" },
        "to": [{ "key_code": "mute" }]
      }]
    }
  ]
}
```

---

## Part 3: Quick Setup Script

Run this to create the Karabiner config:
```bash
cd ~/code/filco && ./setup_karabiner.sh
```

---

## Verification

1. Open **Karabiner-EventViewer**
2. Press each macro key (M1-M10)
3. You should see F13-F22 events
4. Open **Karabiner-Elements** > Complex Modifications
5. Add the "Filco Xacro M10SP Macro Keys" rules

---

## DIP Switch Reference (Back of Keyboard)

| SW | Function |
|----|----------|
| 1 | Swap ESC and ` |
| 2 | Swap Caps Lock and Left Ctrl |
| 3 | Windows/Mac mode (ON = Mac) |
| 4 | Layout: QWERTY/Dvorak/Colemak |

**Recommended for Mac:** SW3 = ON
