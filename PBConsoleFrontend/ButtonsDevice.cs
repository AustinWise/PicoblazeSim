using System;
using System.Collections.Generic;
using System.Text;
using Austin.PicoblazeSim;

namespace Austin.PBConsoleFrontend
{
    class ButtonsDevice : IHardwareDevice
    {
        public IDictionary<byte, Func<byte>> InputDevices
        {
            get
            {
                var dic = new Dictionary<byte, Func<byte>>();
                dic.Add(0x20, () => 0);
                return dic;
            }
        }

        public IDictionary<byte, Action<byte>> OutputDevices
        {
            get { return new Dictionary<byte, Action<byte>>(); }
        }
    }
}
