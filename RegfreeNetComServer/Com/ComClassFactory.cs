using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace RegfreeNetComServer.Com
{
    [ComVisible(true)]
    [SupportedOSPlatform("windows")]
    public class ComClassFactory<T> : ComServer.IClassFactory
    {
        public event EventHandler<ClassFactoryCreateInstanceEventArg<T>>? CreateInstance;

        protected virtual void OnCreateInstance(object sender, ClassFactoryCreateInstanceEventArg<T> e) => CreateInstance?.Invoke(sender, e);

        int ComServer.IClassFactory.LockServer(bool fLock) => 0;
        int ComServer.IClassFactory.CreateInstance(object pUnkOuter, Guid riid, out IntPtr ppvObject)
        {
            var hr = GetValidatedInterfaceType(typeof(T), riid, pUnkOuter, out var interfaceType);
            if (hr != 0)
            {
                ppvObject = IntPtr.Zero;
                return hr;
            }

            var e = new ClassFactoryCreateInstanceEventArg<T>(riid);
            OnCreateInstance(this, e);

            object? obj = e.Instance;
            obj ??= Activator.CreateInstance(typeof(T));

            if (pUnkOuter != null)
            {
                obj = CreateAggregatedObject(pUnkOuter, obj!);
            }

            return GetObjectAsInterface(obj!, interfaceType!, out ppvObject);
        }

        private static int GetValidatedInterfaceType(Type classType, Guid riid, object? outer, out Type? type)
        {
            if (riid == ComServer.IID_IUnknown)
            {
                type = typeof(object);
                return 0;
            }

            if (outer != null)
            {
                type = null;
                return ComServer.CLASS_E_NOAGGREGATION;
            }

            foreach (var iface in classType.GetInterfaces())
            {
                if (iface.GUID == riid)
                {
                    type = iface;
                    return 0;
                }
            }

            type = null;
            return ComServer.E_NOINTERFACE;
        }

        private static int GetObjectAsInterface(object obj, Type interfaceType, out IntPtr interfacePtr)
        {
            if (interfaceType == typeof(object))
            {
                interfacePtr = Marshal.GetIUnknownForObject(obj);
                return 0;
            }

            interfacePtr = Marshal.GetComInterfaceForObject(obj, interfaceType, CustomQueryInterfaceMode.Ignore);
            return interfacePtr == IntPtr.Zero ? ComServer.E_NOINTERFACE : 0;
        }

        private static object? CreateAggregatedObject(object unkOuter, object comObject)
        {
            var outerPtr = Marshal.GetIUnknownForObject(unkOuter);
            try
            {
                return Marshal.GetObjectForIUnknown(Marshal.CreateAggregatedObject(outerPtr, comObject));
            }
            finally
            {
                Marshal.Release(outerPtr);
            }
        }
    }
}
