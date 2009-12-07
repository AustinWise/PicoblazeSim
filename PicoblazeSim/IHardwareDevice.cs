using System;
using System.Collections.Generic;
using System.Text;

namespace Austin.PicoblazeSim
{
    public interface IHardwareDevice
    {
        IDictionary<byte, Func<byte>> InputDevices
        {
            get;
        }

        IDictionary<byte, Action<byte>> OutputDevices
        {
            get;
        }
    }
}
