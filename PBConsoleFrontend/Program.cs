using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Austin.PicoblazeSim;
using Austin.PicoblazeCompile;
using System.Threading;
using System.Windows.Forms;

namespace Austin.PBConsoleFrontend
{
    class Program
    {
        static Dictionary<ushort, uint> iMem = new Dictionary<ushort, uint>();
        static Dictionary<ConsoleKey, byte> consoleKeyToScanCodes = new Dictionary<ConsoleKey, byte>();
        static Form1 frm;


        static byte FB_LADD = 0;
        static byte FB_HADD = 0;
        static byte FB_COLOR = 0;

        static void Main(string[] args)
        {
            loadKeys();

            string program = @"LOAD s0, 03 ;data
LOAD s1, 35 ;addr
STORE s0, s1
FETCH s1, s1
OUTPUT s0, 0c
RETURN C ; does not cause a problem as the flag is not set";


            iMem = Compiler.Compile(new System.IO.StringReader(Properties.Resources.Prog_Rom));
            Cpu cpu = new Cpu(iMem);


            cpu.RegisterInput(0x20, () => 0); // buttons
            cpu.RegisterInput(0x24, () => 1); // switches
            cpu.RegisterInput(0x28, getKey); //keyboard

            cpu.RegisterOutput(0x0A, (data) => FB_LADD = data); //out: lower 8b address to FB
            cpu.RegisterOutput(0x0B, (data) => FB_HADD = data); //out: upper 3b address to FB
            cpu.RegisterOutput(0x0D, (data) => paint(FB_COLOR = data)); //out: color vector out to FB
            cpu.RegisterOutput(0x04, doNothing); //SSEG_EN
            cpu.RegisterOutput(0x08, doNothing); //SSEG_DISP
            cpu.RegisterOutput(0x0C, (data) => Console.Write("LEDs: {0} ", data));

            ThreadPool.QueueUserWorkItem((state) => pumpCpu(cpu));

            frm = new Form1();
            Application.Run(frm);
        }

        private static void pumpCpu(Cpu cpu)
        {
            Thread.Sleep(1000);
            while (true)
            {
                //Console.Write("{0}: ", cpu.State.PC);
                try
                {
                    cpu.Tick();
                    byte[] s = cpu.State.Reg;
                    //Console.WriteLine("s0: {0} s1 {1}", s[0], s[1]);
                    //System.Threading.Thread.Sleep(100);
                }
                catch (NoMoreInstructionsException)
                {
                    break;
                }
            }
        }

        private static void paint(byte color)
        {
            int x = FB_LADD;
            int y = FB_HADD;

            y = y << 1;
            y |= ((x & 0x80) != 0) ? 0x01 : 0x00;
            x &= (0x80 - 1);

            y = y << 1;
            y |= ((x & 0x40) != 0) ? 0x01 : 0x00;
            x &= (0x80 + 0x40 - 1);

            //Console.WriteLine("x: {0} y: {1} color: {2}", x, y, color);

            frm.BeginInvoke(new Action(() => frm.WritePixle(x, y, color)));
        }

        private static void doNothing(byte data)
        {
        }

        private static byte getKey()
        {
            Console.Write("<Press a key>");
            var press = Console.ReadKey(true).Key;
            if (consoleKeyToScanCodes.ContainsKey(press))
                return consoleKeyToScanCodes[press];
            else
                return 0;
        }

        private static void loadKeys()
        {
            foreach (var line in Properties.Resources.KeysToScan.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                string[] splitLine = line.Split(',');
                consoleKeyToScanCodes.Add((ConsoleKey)Enum.Parse(typeof(ConsoleKey), splitLine[0]), byte.Parse(splitLine[1], System.Globalization.NumberStyles.HexNumber));
            }
        }
    }
}
