using System;
using System.Collections.Generic;
using System.Text;
using Austin.PicoblazeSim;
using System.Windows.Forms;

namespace Austin.PBConsoleFrontend
{
    class SwitchesDevice : IHardwareDevice
    {
        public SwitchesDevice()
        {
        }

        public byte Value { get; set; }


        public IDictionary<byte, Func<byte>> InputDevices
        {
            get
            {
                var devs = new Dictionary<byte, Func<byte>>();
                devs.Add(0x24, () => Value);
                return devs;
            }
        }

        public IDictionary<byte, Action<byte>> OutputDevices
        {
            get { return new Dictionary<byte, Action<byte>>(); }
        }
    }
}
