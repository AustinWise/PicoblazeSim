using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Austin.PicoblazeSim;

namespace Austin.PBConsoleFrontend
{
    public partial class frmVga : Form, IHardwareDevice, IInterruptor
    {
        private Dictionary<Keys, byte> consoleKeyToScanCodes = new Dictionary<Keys, byte>();
        private Keys lastKeyPressed;

        public event EventHandler Interrupt;

        private const int pixleSize = 20;

        private byte FB_LADD = 0;
        private byte FB_HADD = 0;

        private byte[] frameBuffer = new byte[0x1000];

        private bool useFrameBuffer = false;
        public bool UseFrameBuffer
        {
            get
            {
                return this.useFrameBuffer;
            }
            set
            {
                this.useFrameBuffer = value;
                this.paintTimer.Enabled = value;
            }
        }

        public frmVga()
        {
            InitializeComponent();
            loadKeys();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private Brush[] brushes = new Brush[0x100];

        public void Clear()
        {
            if (useFrameBuffer)
            {
                for (int i = 0; i < frameBuffer.Length; i++)
                {
                    frameBuffer[i] = 0;
                }
            }
            else
            {
                var g = this.CreateGraphics();
                g.Clear(Color.Black);
                g.Dispose();
            }
        }

        public void WritePixel(int x, int y, byte color)
        {
            Graphics g = this.CreateGraphics();
            WritePixel(x, y, color, g);
            g.Dispose();
        }

        public void WritePixel(int x, int y, byte color, Graphics g)
        {
            x = x * pixleSize;
            y = y * pixleSize;

            Brush pen;
            if (brushes[color] != null)
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
                brushes[color] = pen;
            }

            g.FillRectangle(pen, x, y, pixleSize, pixleSize);
        }

        private int shiftLeftExtend(int val, int places)
        {
            bool lowBit = (val & 0x01) == 1;
            val = val << places;
            if (lowBit)
                val |= ((int)Math.Pow(2, places + 1) - 1);
            return val;
        }

        protected override void OnResize(EventArgs e)
        {
            this.Text = this.Size.ToString();
            base.OnResize(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (this.Interrupt != null)
            {
                this.lastKeyPressed = e.KeyCode;
                this.Interrupt(this, EventArgs.Empty);
            }
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
                dic.Add(0x0D, (data) => paint(data)); //out: color vector out to FB
                return dic;
            }
        }

        private void paint(byte data)
        {
            if (useFrameBuffer)
            {
                frameBuffer[(FB_HADD << 8) | FB_LADD] = data;
            }
            else
            {
                int x = FB_LADD;
                int y = FB_HADD;

                y = y << 1;
                y |= ((x & 0x80) != 0) ? 0x01 : 0x00;
                x &= (0x80 - 1);

                y = y << 1;
                y |= ((x & 0x40) != 0) ? 0x01 : 0x00;
                x &= (0x80 + 0x40 - 1);

                this.BeginInvoke(new Action<int, int, byte>(WritePixel), x, y, data);
            }
        }

        private void loadKeys()
        {
            foreach (var line in Properties.Resources.KeysToScan.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                string[] splitLine = line.Split(',');
                consoleKeyToScanCodes.Add((Keys)Enum.Parse(typeof(Keys), splitLine[0]), byte.Parse(splitLine[1], System.Globalization.NumberStyles.HexNumber));
            }
        }

        private void paintTimer_Tick(object sender, EventArgs e)
        {
            Graphics g = this.CreateGraphics();

            // the framebuffer is expecting a 11-bit address of the format row8:4| col9:4
            for (int x = 0; x < 40; x++)
            {
                for (int y = 0; y < 30; y++)
                {
                    int index = (y << 6) | x;
                    WritePixel(x, y, frameBuffer[index], g);
                }
            }

            g.Dispose();
        }
    }
}
