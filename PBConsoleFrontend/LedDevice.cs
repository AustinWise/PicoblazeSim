using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Austin.PicoblazeSim;

namespace Austin.PBConsoleFrontend
{
    class LedDevice : IHardwareDevice
    {
        public IDictionary<byte, Func<byte>> InputDevices
        {
            get { return new Dictionary<byte, Func<byte>>(); }
        }

        public IDictionary<byte, Action<byte>> OutputDevices
        {
            get
            {
                var dic = new Dictionary<byte, Action<byte>>();

                dic.Add(0x0C, Program.doNothing);

                return dic;
            }
        }
    }
}
