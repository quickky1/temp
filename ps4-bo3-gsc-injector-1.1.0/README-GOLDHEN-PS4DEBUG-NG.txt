PS4 BO3 GSC Injector 1.1.0 - GoldHEN / PS4Debug-NG integration

This source package includes the user-provided PS4Debug-NG v1.3.2 binary at:
PS4 BO3 GSC/Payloads/GoldHEN-13.50/ps4debug-ng_v1.3.2_release_2026-09-20.bin

Usage:
1. On the PS4, enable GoldHEN's payload listener (user reports TCP 9090).
2. Run the WinForms injector on the PC and enter the PS4 IPv4 address; payload port defaults to 9090.
3. Click Send Payload and confirm using the bundled PS4Debug-NG payload. The app sends the file's bytes over TCP; successful transfer is not proof of execution.
4. Check for PS4Debug-NG's on-screen notification. Then use Attach BO3.
5. The included PS4DBG client connects to PS4Debug-NG command TCP 744 and uses async debug TCP 755, matching PS4Debug-NG's documented ports/protocol. The payload listener port 9090 is separate.

Firmware: PS4Debug-NG v1.3.2 documentation lists 13.50 support. Its own README says 13.x patches were validated against decrypted kernels and on-hardware validation to date was on earlier firmware. Treat on-console operation as unverified until confirmed on your console.

BO3 game version/offsets and original GSC injector code were left unchanged. The source project targets .NET Framework 4.8. This archive is source, not a compiled Windows executable; build the solution in Visual Studio with its NuGet dependencies restored.
