using System;
using System.Runtime.InteropServices;

namespace RegfreeNetComServer
{
    [ComImport, Guid("F586D6F4-AF37-441E-80A6-3D33D977882D")]
    public interface IServer
    {
        double ComputePi();
    }
}
