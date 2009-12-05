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

        private static void oldWay()
        {
            load(0, 2);
            load(1, 7);
            add(0, 2);
            addReg(0, 1);
            call((ushort)(iMemPointer + 4));
            load(0, 0);
            iMemPointer += 2;
            addReg(1, 1);
            add(1, 2);
            ret();
        }

        static void ret()
        {
            ins(0x2A, 0);
        }

        static void call(ushort addr)
        {
            ins(0x30, addr);
        }

        static void jump(ushort addr)
        {
            ins(0x34, addr);
        }

        static void load(byte register, byte value)
        {
            ins(0, (ushort)((register << 8) | value));
        }

        static void add(byte register, byte value)
        {
            ins(0x18, (ushort)((register << 8) | value));
        }

        static void addReg(byte rA, byte rB)
        {
            ins(0x19, (ushort)((rA << 8) | (rB << 4)));
        }

        static void ins(byte op, ushort args)
        {
            uint instr = (uint)((op << 12) | args);
            iMem[iMemPointer] = instr;
            iMemPointer++;
        }
    }
}
