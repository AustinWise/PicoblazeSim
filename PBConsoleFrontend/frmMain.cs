using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Austin.PicoblazeSim;
using System.IO;

namespace Austin.PBConsoleFrontend
{
    public partial class frmMain : Form
    {
        private Dictionary<Keys, byte> consoleKeyToScanCodes = new Dictionary<Keys, byte>();

        private Cpu cpu;
        private SwitchesDevice switchDev;
        private KeyboardDevice keyDev;
        private frmVga vgaDev;

        public frmMain()
        {
            InitializeComponent();

            var iMem = new Compiler().Compile(new StringReader(Properties.Resources.Prog_Rom));
            cpu = new Cpu(iMem);

            cpu.RegisterHardwareDevice(switchDev = new SwitchesDevice());
            cpu.RegisterHardwareDevice(vgaDev = new frmVga());
            vgaDev.Show();
            //cpu.RegisterHardwareDevice(keyDev = new KeyboardDevice());
            cpu.RegisterHardwareDevice(new SevenSegmentDevice());
            cpu.RegisterHardwareDevice(new ButtonsDevice());
            cpu.RegisterHardwareDevice(new LedDevice());
        }

        private static void doNothing(byte data)
        {
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            this.cpu.Start();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            this.cpu.Reset();
        }

        private void switches_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            byte res = 0;
            for (int i = 0; i < switches.Items.Count; i++)
            {
                string name = (string)switches.Items[i];
                byte value = byte.Parse(name.Remove(0, 2));
                bool isChecked = e.Index == i ? e.NewValue == CheckState.Checked : switches.GetItemChecked(i);
                res |= (byte)(isChecked ? 0x1 << value : 0x0);
            }
            switchDev.Value = res;
        }
    }
}
