using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading;

namespace RegfreeNetComServer.Com
{
    [SupportedOSPlatform("windows")]
    public partial class ComServer : IDisposable
    {
        private ConcurrentBag<int> _cookies = new();
        private bool _disposedValue;

        public virtual void RegisterClassObject<T>(REGCLS options = REGCLS.REGCLS_MULTIPLEUSE, object? factory = null)
        {
            factory ??= new ComClassFactory<T>();
            ThrowOnError(CoRegisterClassObject(typeof(T).GUID, factory, CLSCTX.CLSCTX_LOCAL_SERVER, options, out var cookie));
            _cookies.Add(cookie);
        }

        public virtual void Resume() => ThrowOnError(CoResumeClassObjects());
        public virtual void Suspend() => ThrowOnError(CoSuspendClassObjects());
        public virtual void Revoke()
        {
            var cookies = Interlocked.Exchange(ref _cookies, new ConcurrentBag<int>());
            foreach (var cookie in cookies)
            {
                _ = CoRevokeClassObject(cookie);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects)
                }

                // free unmanaged resources (unmanaged objects) and override finalizer
                // set large fields to null
                Revoke();
                _disposedValue = true;
            }
        }

        ~ComServer() { Dispose(disposing: false); }
        public void Dispose() { Dispose(disposing: true); GC.SuppressFinalize(this); }

        public static void InitializeSecurity(Guid appID, EOLE_AUTHENTICATION_CAPABILITIES capabilities = EOLE_AUTHENTICATION_CAPABILITIES.EOAC_APPID)
        {
            InitializeSecurity(appID, -1, IntPtr.Zero, 0, 0, IntPtr.Zero, capabilities | EOLE_AUTHENTICATION_CAPABILITIES.EOAC_APPID);
        }

        public static void InitializeSecurity(RPC_C_AUTHN_LEVEL authorizationLevel = RPC_C_AUTHN_LEVEL.RPC_C_AUTHN_LEVEL_DEFAULT, RPC_C_IMP_LEVEL impersonationLevel = RPC_C_IMP_LEVEL.RPC_C_IMP_LEVEL_DEFAULT, EOLE_AUTHENTICATION_CAPABILITIES capabilities = EOLE_AUTHENTICATION_CAPABILITIES.EOAC_NONE)
            => InitializeSecurity(IntPtr.Zero, 0, IntPtr.Zero, authorizationLevel, impersonationLevel, IntPtr.Zero, capabilities);

        public static void InitializeSecurity(IntPtr securityDescriptor, int authorizationServicesCount, IntPtr authorizationServices, RPC_C_AUTHN_LEVEL authorizationLevel, RPC_C_IMP_LEVEL impersonationLevel, IntPtr authenticationList, EOLE_AUTHENTICATION_CAPABILITIES capabilities)
            => ThrowOnError(CoInitializeSecurity(securityDescriptor, authorizationServicesCount, authorizationServices, IntPtr.Zero, authorizationLevel, impersonationLevel, authenticationList, capabilities, IntPtr.Zero));

        public static void InitializeSecurity(Guid securityDescriptor, int authorizationServicesCount, IntPtr authorizationServices, RPC_C_AUTHN_LEVEL authorizationLevel, RPC_C_IMP_LEVEL impersonationLevel, IntPtr authenticationList, EOLE_AUTHENTICATION_CAPABILITIES capabilities)
            => ThrowOnError(CoInitializeSecurity(securityDescriptor, authorizationServicesCount, authorizationServices, IntPtr.Zero, authorizationLevel, impersonationLevel, authenticationList, capabilities, IntPtr.Zero));

        internal static void ThrowOnError(int value)
        {
            if (value < 0)
                Marshal.ThrowExceptionForHR(value);
        }

        internal const int CLASS_E_NOAGGREGATION = unchecked((int)0x80040110);
        internal const int E_NOINTERFACE = unchecked((int)0x80004002);
        internal static Guid IID_IUnknown = new("00000000-0000-0000-c000-000000000046");

        [DllImport("ole32")]
        private static extern int CoRegisterClassObject([MarshalAs(UnmanagedType.LPStruct)] Guid guid, [MarshalAs(UnmanagedType.IUnknown)] object obj, CLSCTX context, REGCLS flags, out int register);

        [DllImport("ole32")]
        private static extern int CoResumeClassObjects();

        [DllImport("ole32")]
        private static extern int CoSuspendClassObjects();

        [DllImport("ole32")]
        private static extern int CoRevokeClassObject(int register);

        [DllImport("ole32")]
        private static extern int CoInitializeSecurity(IntPtr pSecDesc, int cAuthSvc, IntPtr asAuthSvc, IntPtr pReserved1, RPC_C_AUTHN_LEVEL dwAuthnLevel, RPC_C_IMP_LEVEL dwImpLevel, IntPtr pAuthList, EOLE_AUTHENTICATION_CAPABILITIES dwCapabilities, IntPtr pReserved3);

        [DllImport("ole32")]
        private static extern int CoInitializeSecurity([MarshalAs(UnmanagedType.LPStruct)] Guid pSecDesc, int cAuthSvc, IntPtr asAuthSvc, IntPtr pReserved1, RPC_C_AUTHN_LEVEL dwAuthnLevel, RPC_C_IMP_LEVEL dwImpLevel, IntPtr pAuthList, EOLE_AUTHENTICATION_CAPABILITIES dwCapabilities, IntPtr pReserved3);

        [ComImport, Guid("00000001-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IClassFactory
        {
            [PreserveSig]
            int CreateInstance([MarshalAs(UnmanagedType.Interface)] object pUnkOuter, [MarshalAs(UnmanagedType.LPStruct)] Guid riid, out IntPtr ppvObject);

            [PreserveSig]
            int LockServer(bool fLock);
        }
    }
}
