using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Austin.PicoblazeSim;
using Austin.PicoblazeCompile;

namespace Austin.PBConsoleFrontend
{
    class Program
    {
        static Dictionary<ushort, uint> iMem = new Dictionary<ushort, uint>();
        static Dictionary<ConsoleKey, byte> consoleKeyToScanCodes = new Dictionary<ConsoleKey, byte>();

        static void Main(string[] args)
        {
            loadKeys();

            string program = @"LOAD s0, 3 ;data
LOAD s1, 35 ;addr
STORE s0, s1
FETCH s1, s1
OUTPUT s0, 0c";


            iMem = Compiler.Compile(new System.IO.StringReader(program));
            Cpu cpu = new Cpu(iMem);

            cpu.RegisterInput(0x28, getKey);
            cpu.RegisterOutput(0x0C, (data) => Console.Write("LEDs: {0} ", data));

            while (true)
            {
                Console.Write("{0}: ", cpu.State.PC);
                try
                {
                    cpu.Tick();
                    byte[] s = cpu.State.Reg;
                    Console.WriteLine("s0: {0} s1 {1}", s[0], s[1]);
                    System.Threading.Thread.Sleep(100);
                }
                catch (NoMoreInstructionsException)
                {
                    break;
                }
            }
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
