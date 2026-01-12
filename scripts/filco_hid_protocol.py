#!/usr/bin/env python3
import hid
import sys
import time
import struct

VENDOR_ID = 0x2efd
PRODUCT_ID = 0x1972

def enumerate_devices():
    devices = hid.enumerate(VENDOR_ID, PRODUCT_ID)
    print(f"Found {len(devices)} interfaces:")
    for i, dev in enumerate(devices):
        path = dev['path'].decode() if isinstance(dev['path'], bytes) else dev['path']
        print(f"  [{i}] Usage Page: 0x{dev['usage_page']:04x}, Usage: 0x{dev['usage']:04x}, Interface: {dev['interface_number']}")
    return devices

def open_vendor_interface(devices, usage_page_filter=None):
    for dev in devices:
        usage_page = dev['usage_page']
        if usage_page_filter and usage_page != usage_page_filter:
            continue
        if usage_page >= 0xFF00:
            try:
                device = hid.device()
                device.open_path(dev['path'])
                print(f"Opened vendor interface: usage_page=0x{usage_page:04x}")
                return device, dev
            except IOError as e:
                print(f"Failed to open 0x{usage_page:04x}: {e}")
    return None, None

def send_feature_report(device, report_id, data):
    report = bytes([report_id]) + data
    print(f"Sending feature report: {' '.join(f'{b:02x}' for b in report[:32])}...")
    try:
        result = device.send_feature_report(report)
        print(f"  Result: {result}")
        return result
    except IOError as e:
        print(f"  Error: {e}")
        return -1

def get_feature_report(device, report_id, size=64):
    print(f"Getting feature report ID={report_id}...")
    try:
        data = device.get_feature_report(report_id, size)
        print(f"  Got {len(data)} bytes: {' '.join(f'{b:02x}' for b in data[:32])}")
        return data
    except IOError as e:
        print(f"  Error: {e}")
        return None

def probe_report_ids(device):
    print("\n--- Probing Feature Report IDs ---")
    valid_ids = []
    for report_id in range(0, 256):
        try:
            data = device.get_feature_report(report_id, 64)
            if data:
                print(f"Report ID {report_id} (0x{report_id:02x}): {len(data)} bytes")
                print(f"  Data: {' '.join(f'{b:02x}' for b in data[:32])}")
                valid_ids.append(report_id)
        except:
            pass
    return valid_ids

def monitor_input(device, duration=10):
    print(f"\n--- Monitoring for {duration}s (press macro keys) ---")
    device.set_nonblocking(True)
    start = time.time()
    while time.time() - start < duration:
        try:
            data = device.read(64)
            if data:
                print(f"[INPUT] {' '.join(f'{b:02x}' for b in data)}")
        except:
            pass
        time.sleep(0.01)

def try_init_commands(device):
    print("\n--- Trying initialization commands ---")
    
    common_report_ids = [0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x10, 0x20, 0x30, 0x40, 0x50, 0x60, 0x80, 0xA0, 0xC0, 0xE0, 0xF0, 0xFF]
    
    for rid in common_report_ids:
        try:
            data = device.get_feature_report(rid, 64)
            if data and len(data) > 1:
                print(f"Report 0x{rid:02x}: {' '.join(f'{b:02x}' for b in data[:16])}...")
        except:
            pass

def main():
    print("=== FILCO Xacro M10SP Protocol Probe ===\n")
    
    devices = enumerate_devices()
    if not devices:
        print("No keyboard found!")
        return
    
    vendor_devs = [d for d in devices if d['usage_page'] >= 0xFF00]
    print(f"\nVendor-specific interfaces: {len(vendor_devs)}")
    
    for vdev in vendor_devs:
        usage_page = vdev['usage_page']
        print(f"\n{'='*50}")
        print(f"Testing interface: usage_page=0x{usage_page:04x}")
        print('='*50)
        
        try:
            device = hid.device()
            device.open_path(vdev['path'])
        except IOError as e:
            print(f"Cannot open: {e}")
            continue
        
        try_init_commands(device)
        
        valid_ids = probe_report_ids(device)
        print(f"\nValid report IDs found: {valid_ids}")
        
        print("\nMonitoring input - press M1-M10 keys...")
        monitor_input(device, duration=15)
        
        device.close()

if __name__ == "__main__":
    main()
