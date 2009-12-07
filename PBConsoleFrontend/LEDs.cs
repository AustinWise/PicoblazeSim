using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Austin.PicoblazeSim;

namespace Austin.PBConsoleFrontend
{
    public partial class LEDs : UserControl, IHardwareDevice
    {
        public LEDs()
        {
            InitializeComponent();
        }

        private void setLeds(byte data)
        {
            chk0.Checked = (data & 0x1) != 0;
            chk1.Checked = (data & 0x2) != 0;
            chk2.Checked = (data & 0x4) != 0;
            chk3.Checked = (data & 0x8) != 0;
            chk4.Checked = (data & 0x10) != 0;
            chk5.Checked = (data & 0x20) != 0;
            chk6.Checked = (data & 0x40) != 0;
            chk7.Checked = (data & 0x80) != 0;
        }

        public IDictionary<byte, Func<byte>> InputDevices
        {
            get
            {
                return new Dictionary<byte, Func<byte>>();
            }
        }

        public IDictionary<byte, Action<byte>> OutputDevices
        {
            get
            {
                var dic = new Dictionary<byte, Action<byte>>();
                dic.Add(0x0C, (data) => this.BeginInvoke(new Action<byte>(setLeds), data));
                return dic;
            }
        }
    }
}
