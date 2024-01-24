using System;

namespace RegfreeNetComServer.Com
{
    public class ClassFactoryCreateInstanceEventArg<T> : EventArgs
    {
        public ClassFactoryCreateInstanceEventArg(Guid riid)
        {
            InterfaceId = riid;
        }

        public Guid InterfaceId { get; }
        public T? Instance { get; set; }
    }
}
