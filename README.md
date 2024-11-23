# RegfreeNetComServer
A registry-free Out-Of-Process COM server that also demonstrates 32-64 bit communication

The sample is composed of
* a .NET 8.0 COM Out-Of-Process server (meant to be run in x64 but that could be changed easily in the project's build configuration)
* a native x86, x64 and ARM64 console client written in C++
* a simple VBScript client.

Everything is reg-free. All clients can call the .NET Server.

## Notes
Reg-Free is here based on a .TLB file (server.tlb) present aside the server .exe and the client .exe (no TLB is embedded in this sample). Both .NET and C++ projects have an app.manifest that declares that tlb.

The .NET server builds this TLB from an IDL files, as its .csproj has been enhanced to do that automatically.
The native client needs that tlb to run. Its build is dependent on at least one .NET project build.
