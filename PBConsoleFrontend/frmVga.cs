using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Austin.PicoblazeSim;

namespace Austin.PBConsoleFrontend
{
    public partial class frmVga : Form, IHardwareDevice, IInterruptor
    {
        private Dictionary<Keys, byte> consoleKeyToScanCodes = new Dictionary<Keys, byte>();
        private Keys lastKeyPressed;

        private const int pixleSize = 20;

        private static byte FB_LADD = 0;
        private static byte FB_HADD = 0;

        public frmVga()
        {
            InitializeComponent();
            loadKeys();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private Dictionary<byte, Brush> brushes = new Dictionary<byte, Brush>();

        public void Clear()
        {
            Graphics g = this.CreateGraphics();
            g.Clear(Color.Black);
            g.Dispose();

        }

        public void WritePixle(int x, int y, byte color)
        {
            x = x * pixleSize;
            y = y * pixleSize;

            Brush pen;
            if (brushes.ContainsKey(color))
                pen = brushes[color];
            else
            {
                int red = (color >> 5);
                red = shiftLeftExtend(red, 5);
                int green = ((color >> 2) & 0x7);
                green = shiftLeftExtend(green, 5);
                int blue = (color & 0x3);
                blue = shiftLeftExtend(blue, 6);
                pen = new SolidBrush(Color.FromArgb(red, green, blue));
                brushes.Add(color, pen);
            }


            Graphics g = this.CreateGraphics();
            g.FillRectangle(pen, x, y, pixleSize, pixleSize);
            g.Dispose();
        }

        private int shiftLeftExtend(int val, int places)
        {
            bool lowBit = (val & 0x01) == 1;
            val = val << places;
            if (lowBit)
                val |= ((int)Math.Pow(2, places + 1) - 1);
            return val;
        }

        private void paint(byte color)
        {
            int x = FB_LADD;
            int y = FB_HADD;

            y = y << 1;
            y |= ((x & 0x80) != 0) ? 0x01 : 0x00;
            x &= (0x80 - 1);

            y = y << 1;
            y |= ((x & 0x40) != 0) ? 0x01 : 0x00;
            x &= (0x80 + 0x40 - 1);

            this.BeginInvoke(new Action(() => WritePixle(x, y, color)));
        }

        protected override void OnResize(EventArgs e)
        {
            this.Text = this.Size.ToString();
            base.OnResize(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            this.lastKeyPressed = e.KeyCode;
            this.Interrupt(this, EventArgs.Empty);
            base.OnKeyDown(e);
        }

        private byte getKey()
        {
            var press = lastKeyPressed;
            if (consoleKeyToScanCodes.ContainsKey(lastKeyPressed))
                return consoleKeyToScanCodes[lastKeyPressed];
            else
                return 0;
        }

        public IDictionary<byte, Func<byte>> InputDevices
        {
            get
            {
                var dic = new Dictionary<byte, Func<byte>>();
                dic.Add(0x28, new Func<byte>(getKey));
                return dic;
            }
        }

        public IDictionary<byte, Action<byte>> OutputDevices
        {
            get
            {
                var dic = new Dictionary<byte, Action<byte>>();
                dic.Add(0x0A, (data) => FB_LADD = data); //out: lower 8b address to FB
                dic.Add(0x0B, (data) => FB_HADD = data); //out: upper 3b address to FB
                dic.Add(0x0D, new Action<byte>(paint)); //out: color vector out to FB
                return dic;
            }
        }

        public event EventHandler Interrupt;

        private void loadKeys()
        {
            foreach (var line in Properties.Resources.KeysToScan.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                string[] splitLine = line.Split(',');
                consoleKeyToScanCodes.Add((Keys)Enum.Parse(typeof(Keys), splitLine[0]), byte.Parse(splitLine[1], System.Globalization.NumberStyles.HexNumber));
            }
        }
    }
}
