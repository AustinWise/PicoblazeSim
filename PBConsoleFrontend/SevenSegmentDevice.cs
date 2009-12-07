using System;
using System.Collections.Generic;
using System.Text;
using Austin.PicoblazeSim;

namespace Austin.PBConsoleFrontend
{
    class SevenSegmentDevice : IHardwareDevice
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

                dic.Add(0x04, Program.doNothing); //SSEG_EN
                dic.Add(0x08, Program.doNothing); //SSEG_DISP

                return dic;
            }
        }
    }
}
