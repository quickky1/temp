BO3 MEMORY DUMPER ADD-ON

This source package adds a "Dump BO3 Memory" button to the existing PS4 BO3 GSC Injector WinForms app.

Usage:
1. Build/open the existing solution using Visual Studio with the .NET desktop development workload and .NET Framework 4.8 targeting pack.
2. Start BO3 on your own PS4 and connect the existing PS4Debug-NG connection (debug port, not GoldHEN payload upload port 9090).
3. Attach to eboot.bin using the app's existing Attach button.
4. Click Dump BO3 Memory and select a destination folder with ample free disk space. A full readable process dump can be several GB.
5. The app creates BO3_CUSA02290_memory_<timestamp>, with raw region_*.bin files and dump_manifest.txt. ZIP that folder and share it for analysis.

Dump format:
- Each region file corresponds to one readable virtual memory mapping returned by PS4Debug-NG.
- Region files preserve address alignment. Failed 1 MiB reads are zero-filled, and the exact address/error is listed in dump_manifest.txt.
- Non-readable mappings are skipped and recorded.
- This is a raw process-memory dump, not a guaranteed save-data or GobbleGum-only extraction. The manifest is required to interpret the region files.

No game memory is written by this dumper. It only calls GetProcessMaps and ReadMemory from the existing PS4Debug library.

Build status: source change only. This environment does not have Visual Studio/MSBuild or the Windows .NET Framework targeting pack, so no compiled Windows EXE is claimed or included.
