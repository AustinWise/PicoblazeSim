using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private Compiler comp = new Compiler();
        private SwitchesDevice switchDev;
        private frmVga vgaDev;

        public frmMain()
        {
            InitializeComponent();

            vgaDev = new frmVga();
            vgaDev.Show();
            switchDev = new SwitchesDevice();
            chkUseFrameBuffer.Checked = vgaDev.UseFrameBuffer;

            this.txtSrc.Text = Properties.Resources.Prog_Rom;
        }

        private static void doNothing(byte data)
        {
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Dictionary<ushort, uint> iMem;
            try
            {
                iMem = comp.Compile(new StringReader(txtSrc.Text));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed to compile.");
                return;
            }

            btnStart.Enabled = false;
            chkUseFrameBuffer.Enabled = false;

            cpu = new Cpu(iMem);

            cpu.RegisterHardwareDevice(switchDev);
            cpu.RegisterHardwareDevice(vgaDev);
            cpu.RegisterHardwareDevice(new SevenSegmentDevice());
            cpu.RegisterHardwareDevice(new ButtonsDevice());
            cpu.RegisterHardwareDevice(new LedDevice());

            this.cpu.Start();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            this.btnStart.Enabled = true;
            this.chkUseFrameBuffer.Enabled = true;
            this.cpu.Reset();
            vgaDev.Clear();
            lblInstructionsPerSec.Text = string.Format("{0:.###} MHz", cpu.InstructionsPerSecond / 1000000);
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

        private void chkUseFrameBuffer_CheckedChanged(object sender, EventArgs e)
        {
            vgaDev.UseFrameBuffer = chkUseFrameBuffer.Checked;
        }
    }
}
