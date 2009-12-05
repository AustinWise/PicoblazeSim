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
        static ushort iMemPointer = 0;

        static void Main(string[] args)
        {
            string program = @"LOAD s0, 2 ;test
LOAD s1, 07
add s0, 02
add s0, s1";

            iMem = Compiler.Compile(new System.IO.StringReader(program));


            Cpu cpu = new Cpu(iMem);

            while (true)
            {
                byte[] s = cpu.State.Reg;
                Console.WriteLine("{2}: s0: {0} s1 {1}", s[0], s[1], cpu.State.PC);
                try
                {
                    cpu.Tick();
                    System.Threading.Thread.Sleep(100);
                }
                catch (NoMoreInstructionsException)
                {
                    break;
                }
            }
        }
    }
}
