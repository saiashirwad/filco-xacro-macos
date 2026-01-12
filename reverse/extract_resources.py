#!/usr/bin/env python3
"""Extract embedded resources from .NET assembly"""
import os
import sys
import struct

def find_resources_in_pe(filepath):
    """Find .NET resources embedded in PE file"""
    with open(filepath, 'rb') as f:
        data = f.read()
    
    # Look for .NET resource markers
    # FILCO_Assist MSI should be embedded as a resource
    
    # Search for MSI magic number (compound document)
    msi_magic = b'\xD0\xCF\x11\xE0\xA1\xB1\x1A\xE1'
    
    offsets = []
    pos = 0
    while True:
        pos = data.find(msi_magic, pos)
        if pos == -1:
            break
        offsets.append(pos)
        print(f"Found MSI/OLE compound document at offset: 0x{pos:X} ({pos})")
        pos += 1
    
    return data, offsets

def extract_msi(data, offset, output_path):
    """Extract MSI file from data"""
    # Read the OLE compound document
    # The size is encoded in the header
    
    # For now, try to find the end by looking for the next structure
    # or just extract a large chunk
    
    # OLE header is at offset, sector size at offset+30 (2 bytes)
    sector_size_power = struct.unpack('<H', data[offset+30:offset+32])[0]
    sector_size = 2 ** sector_size_power
    print(f"Sector size: {sector_size}")
    
    # Number of FAT sectors at offset+44 (4 bytes)
    fat_sectors = struct.unpack('<I', data[offset+44:offset+48])[0]
    print(f"FAT sectors: {fat_sectors}")
    
    # Total sectors at offset+48 (4 bytes) for v4, or calculate
    # This is complex, let's just extract a large chunk and let Windows tools parse it
    
    # Try to find a reasonable end - scan for patterns that shouldn't be in MSI
    # Or just extract until we find another header or EOF
    
    # Simple approach: extract until next MSI header or end of .NET resource region
    next_pos = len(data)  # default to end
    
    # Look for common markers that indicate end of MSI
    # MSI files typically end with padding
    
    # For simplicity, let's extract a reasonable size based on typical FILCO Assist size
    # The installer was ~19MB, the MSI inside is probably ~17MB
    chunk_size = 18 * 1024 * 1024  # 18MB max
    end_pos = min(offset + chunk_size, len(data))
    
    msi_data = data[offset:end_pos]
    
    with open(output_path, 'wb') as f:
        f.write(msi_data)
    
    print(f"Extracted {len(msi_data)} bytes to {output_path}")
    return output_path

if __name__ == '__main__':
    exe_path = '/Users/texoport/code/filco/reverse/FILCOAssistSetup_2.0.97.exe'
    output_dir = '/Users/texoport/code/filco/reverse/msi_extracted'
    
    os.makedirs(output_dir, exist_ok=True)
    
    data, offsets = find_resources_in_pe(exe_path)
    
    for i, offset in enumerate(offsets):
        output_path = os.path.join(output_dir, f'embedded_{i}.msi')
        extract_msi(data, offset, output_path)
        
        # Verify with file command
        os.system(f'file "{output_path}"')
