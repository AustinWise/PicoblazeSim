using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Austin.PBConsoleFrontend
{
    public partial class Form1 : Form
    {
        private const int pixleSize = 20;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Program.cpu.Start();
        }

        private Dictionary<byte, Brush> brushes = new Dictionary<byte, Brush>();

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
        }

        private int shiftLeftExtend(int val, int places)
        {
            bool lowBit = (val & 0x01) == 1;
            val = val << places;
            if (lowBit)
                val |= ((int)Math.Pow(2, places + 1) - 1);
            return val;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Program.lastKeyPressed = e.KeyData;
            Program.cpu.Interrupt();
        }
    }
}
