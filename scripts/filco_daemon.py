#!/usr/bin/env python3
import hid
import time
import sys
import os
import signal
import struct
from threading import Thread, Event

VENDOR_ID = 0x2efd
PRODUCT_ID = 0x1972

VENDOR_USAGE_PAGES = [0xffa0, 0xffff]

INIT_SEQUENCES = [
    bytes([0x00] * 64),
    bytes([0x01] + [0x00] * 63),
    bytes([0x02] + [0x00] * 63),
    bytes([0x05] + [0x00] * 63),
    bytes([0x06] + [0x00] * 63),
    bytes([0x07] + [0x00] * 63),
    bytes([0x08] + [0x00] * 63),
    bytes([0x0A] + [0x00] * 63),
    bytes([0x10] + [0x00] * 63),
    bytes([0xA0] + [0x00] * 63),
    bytes([0xB0] + [0x00] * 63),
    bytes([0xC0] + [0x00] * 63),
    bytes([0xE0] + [0x00] * 63),
    bytes([0xF0] + [0x00] * 63),
    struct.pack("<II", 0x5AA555AA, 0xCC3300FF) + bytes([0x00] * 56),
    struct.pack("<II", 0xAA55A55A, 0xFF0033CC) + bytes([0x00] * 56),
]

stop_event = Event()

def signal_handler(sig, frame):
    print("\nShutting down...")
    stop_event.set()

def enumerate_filco():
    devices = hid.enumerate(VENDOR_ID, PRODUCT_ID)
    print(f"Found {len(devices)} HID interfaces")
    
    vendor_interfaces = []
    for dev in devices:
        usage_page = dev['usage_page']
        path = dev['path']
        print(f"  Interface: usage_page=0x{usage_page:04x}, usage=0x{dev['usage']:04x}")
        
        if usage_page in VENDOR_USAGE_PAGES or usage_page >= 0xFF00:
            vendor_interfaces.append(dev)
    
    return vendor_interfaces

def try_feature_reports(device):
    print("\nProbing feature reports...")
    valid_reports = []
    
    for report_id in range(256):
        for size in [8, 16, 32, 64, 128, 256]:
            try:
                data = device.get_feature_report(report_id, size)
                if data and len(data) > 1 and any(b != 0 for b in data[1:]):
                    print(f"  Report 0x{report_id:02x} ({size}b): {' '.join(f'{b:02x}' for b in data[:16])}")
                    valid_reports.append((report_id, data))
                    break
            except:
                pass
    
    return valid_reports

def try_init_sequences(device):
    print("\nTrying initialization sequences...")
    
    for i, seq in enumerate(INIT_SEQUENCES):
        try:
            result = device.send_feature_report(seq)
            print(f"  Seq {i}: sent, result={result}")
            time.sleep(0.05)
            
            try:
                response = device.get_feature_report(seq[0], 64)
                if response and any(b != 0 for b in response):
                    print(f"    Response: {' '.join(f'{b:02x}' for b in response[:16])}")
            except:
                pass
        except Exception as e:
            pass

def monitor_interface(device_info, timeout=30):
    path = device_info['path']
    usage_page = device_info['usage_page']
    
    print(f"\n{'='*50}")
    print(f"Monitoring interface 0x{usage_page:04x}")
    print(f"Press macro keys M1-M10 now...")
    print(f"{'='*50}")
    
    try:
        device = hid.device()
        device.open_path(path)
        device.set_nonblocking(True)
    except IOError as e:
        print(f"Cannot open: {e}")
        return
    
    try_feature_reports(device)
    try_init_sequences(device)
    
    print(f"\nListening for {timeout}s...")
    start = time.time()
    while time.time() - start < timeout and not stop_event.is_set():
        try:
            data = device.read(64)
            if data:
                hex_str = ' '.join(f'{b:02x}' for b in data)
                print(f"[INPUT 0x{usage_page:04x}] {hex_str}")
        except:
            pass
        time.sleep(0.001)
    
    device.close()

def main():
    signal.signal(signal.SIGINT, signal_handler)
    
    print("=" * 60)
    print(" FILCO Xacro M10SP Macro Key Daemon")
    print("=" * 60)
    
    vendor_interfaces = enumerate_filco()
    
    if not vendor_interfaces:
        print("\nNo vendor-specific interfaces found!")
        return
    
    print(f"\nFound {len(vendor_interfaces)} vendor interface(s)")
    
    for vi in vendor_interfaces:
        if stop_event.is_set():
            break
        monitor_interface(vi, timeout=20)
    
    print("\nDone.")

if __name__ == "__main__":
    main()
