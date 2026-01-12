#!/usr/bin/env python3
"""
Filco Xacro M10SP HID Probe
Detects all HID interfaces exposed by the keyboard and tries to read events.
"""

import hid
import sys
import time

# Filco Xacro M10SP identifiers
VENDOR_ID = 0x2efd
PRODUCT_ID = 0x1972

def list_all_filco_interfaces():
    """List all HID interfaces exposed by the Filco keyboard."""
    print("=" * 60)
    print("Scanning for Filco Xacro M10SP interfaces...")
    print("=" * 60)
    
    devices = hid.enumerate(VENDOR_ID, PRODUCT_ID)
    
    if not devices:
        print(f"\n[ERROR] No devices found with VID:0x{VENDOR_ID:04x} PID:0x{PRODUCT_ID:04x}")
        print("Make sure the keyboard is connected.")
        return []
    
    print(f"\nFound {len(devices)} interface(s):\n")
    
    for i, dev in enumerate(devices):
        print(f"Interface {i+1}:")
        print(f"  Path:          {dev['path'].decode() if isinstance(dev['path'], bytes) else dev['path']}")
        print(f"  Usage Page:    0x{dev['usage_page']:04x} ({dev['usage_page']})")
        print(f"  Usage:         0x{dev['usage']:04x} ({dev['usage']})")
        print(f"  Interface:     {dev['interface_number']}")
        print(f"  Product:       {dev['product_string']}")
        print(f"  Manufacturer:  {dev['manufacturer_string']}")
        print(f"  Serial:        {dev['serial_number']}")
        
        # Identify interface type
        usage_page = dev['usage_page']
        usage = dev['usage']
        
        if usage_page == 0x01:  # Generic Desktop
            if usage == 0x06:
                print(f"  Type:          Standard Keyboard")
            elif usage == 0x02:
                print(f"  Type:          Mouse")
            elif usage == 0x01:
                print(f"  Type:          Pointer")
            else:
                print(f"  Type:          Generic Desktop (usage {usage})")
        elif usage_page == 0x0C:  # Consumer
            print(f"  Type:          Consumer Controls (Media Keys)")
        elif usage_page >= 0xFF00:  # Vendor-specific
            print(f"  Type:          VENDOR-SPECIFIC (likely macro keys!)")
        else:
            print(f"  Type:          Unknown (page: 0x{usage_page:04x})")
        
        print()
    
    return devices


def try_read_interface(device_info, timeout_ms=5000):
    """Try to open and read from a specific interface."""
    path = device_info['path']
    usage_page = device_info['usage_page']
    
    print(f"\nAttempting to read from interface (usage_page=0x{usage_page:04x})...")
    print("Press macro keys on the keyboard within 5 seconds...")
    print("-" * 40)
    
    try:
        device = hid.device()
        device.open_path(path)
        device.set_nonblocking(False)
        
        print(f"[OK] Opened device successfully")
        
        start_time = time.time()
        while time.time() - start_time < (timeout_ms / 1000):
            try:
                data = device.read(64, timeout_ms=1000)
                if data:
                    print(f"\n[DATA] Received {len(data)} bytes:")
                    hex_str = ' '.join(f'{b:02x}' for b in data)
                    print(f"  HEX: {hex_str}")
                    print(f"  RAW: {data}")
                    return True
            except Exception as e:
                pass
        
        print("[TIMEOUT] No data received")
        device.close()
        return False
        
    except IOError as e:
        print(f"[ERROR] Cannot open device: {e}")
        print("  This is normal for the main keyboard interface (macOS claims it)")
        return False


def main():
    print("\n" + "=" * 60)
    print(" FILCO XACRO M10SP HID INTERFACE PROBE")
    print("=" * 60)
    
    devices = list_all_filco_interfaces()
    
    if not devices:
        return
    
    # Find vendor-specific interfaces (most likely macro keys)
    vendor_specific = [d for d in devices if d['usage_page'] >= 0xFF00]
    
    if vendor_specific:
        print("\n" + "=" * 60)
        print(" FOUND VENDOR-SPECIFIC INTERFACE(S) - LIKELY MACRO KEYS!")
        print("=" * 60)
        
        for dev in vendor_specific:
            try_read_interface(dev)
    else:
        print("\n[INFO] No vendor-specific interfaces found.")
        print("The macro keys might use standard HID codes or need initialization.")
        
        # Try consumer page
        consumer = [d for d in devices if d['usage_page'] == 0x0C]
        if consumer:
            print("\nTrying Consumer Control interface...")
            for dev in consumer:
                try_read_interface(dev)
    
    print("\n" + "=" * 60)
    print(" SUMMARY")
    print("=" * 60)
    print("""
Next steps based on results:

1. If VENDOR-SPECIFIC data was captured:
   - The macro keys send proprietary HID reports
   - We can create a daemon to translate these to key events

2. If no data from vendor-specific interfaces:
   - Macro keys might need hardware initialization
   - Try: Press Fn+Ctrl to enter macro mode, then exit
   - Check DIP switch settings

3. If only standard keyboard events:
   - Use Karabiner-Elements to remap directly

Run with --monitor to continuously monitor all interfaces.
""")


if __name__ == "__main__":
    main()
