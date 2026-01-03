# RegfreeNetComServer
A registry-free Out-Of-Process COM server that also demonstrates 32-64 bit communication

The sample is composed of
* a .NET 10.0 COM Out-Of-Process server (meant to be run in x64 but that could be changed easily in the project's build configuration)
* a native x86, x64 and ARM64 console client written in C++
* a simple VBScript client.

Everything is reg-free. All clients can call the .NET Server.

## Notes
Reg-Free COM is here based on a Type Library .TLB file (`server.tlb`) present aside the server .exe and the client .exe (no TLB is embedded in this sample). Both .NET and C++ projects have a specific `app.manifest` file that declares that type library.

That type library is necessary for COM to marshal the `IServer` COM interface between the COM server and its clients. If the sample was using a well-known interface (for example `IOleCommandTarget` - I used this one a lot because it's pretty generic), then it would not be necessary as the tlb would already be known.

If the `IServer` COM interface was *IUnknown*-derived instead of *IDispatch*-derived, or/and if it was using non COM-Automation compatible types (`BSTR`, `VARIANT`, `double`, etc. are COM-compatible types), then we would need to use custom proxy and stubs. They are not needed here because we use `IDispatch` and restrict ourselves to COM-Automation types. It's much easier to do so. It happens magically because `OleAut32.dll` builds a proxy and a stub dynamically from the tlb.

All this works with VBScript as VBScript only uses the server's `IDispatch` implementation ("late-binding"), not the `IServer` one ("early-binding"). One can do the same with a native client (no requirement for TLB), but using `IServer` is easier from C/C++ (or other native languages for that matter).

Sadly, there's no tooling in Visual Studio C# projects to build .tlb files from .idl (there is only in Visual Studio C++ projects!), so the C# project references an extra `GetWindowsSDKPaths.targets` file (to gather various Windows SDK and Visual studio paths), plus some extra lines in the `.csproj` to do that automatically.

The native client needs the `server.tlb` to run. Its build is dependent on at least one .NET project successful build.
