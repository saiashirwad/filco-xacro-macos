# Next Steps: USB Protocol Capture

If a pure software-driven (no hardware programming) solution is still desired, the next phase must involve live packet capture.

## Required Environment
-   **Windows VM** (Parallels, VMware, or VirtualBox).
-   **USB Passthrough**: The Filco keyboard must be connected directly to the Windows guest.
-   **FILCO Assist Software**: Installed and running on the Windows guest.

## Capture Workflow
1.  **Install Wireshark & USBPcap** on the Windows guest.
2.  **Start Capture**: Filter by `usb.device_address == <address>` or use the USBPcap interface.
3.  **Perform Actions**:
    -   Open FILCO Assist (Capture the "Init" sequence).
    -   Remap a key (Capture the `SetKeyDefinitionTable` sequence).
    -   Press a macro key (Capture the `0xFFA0` report sequence).
4.  **Analyze**:
    -   Identify the **Feature Report ID** and payload used to "Unlock" the macro keys.
    -   Export the bytes to a `.pcap` or text file.

## Implementation on macOS
Once the bytes are known:
1.  Use the `filco_daemon.py` script.
2.  Add the captured bytes to the `INIT_SEQUENCES` list.
3.  Send the `send_feature_report` command.
4.  Monitor the input stream on the `0xFFA0` interface.
