using System;
using System.Collections.Generic;
using System.Text;
using Austin.PicoblazeSim;
using System.Threading;
using System.Windows.Forms;

namespace Austin.PBConsoleFrontend
{
    class Program
    {
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.Run(new frmMain());
        }

        public static void doNothing(byte data)
        {
        }
    }
}
