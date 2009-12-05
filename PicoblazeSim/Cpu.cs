using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Austin.PicoblazeSim
{
    public class Cpu
    {
        public Cpu(Dictionary<ushort, uint> iMem)
        {
            this.instructionMemory = iMem;
            this.State = new CpuState();
        }

        private Dictionary<ushort, uint> instructionMemory;
        private Operations ops = new Operations();


        public void RegisterInput(byte id, Func<byte> device)
        {
            State.InputDevices.Add(id, device);
        }

        public void RegisterOutput(byte id, Action<byte> device)
        {
            State.OutputDevices.Add(id, device);
        }


        public CpuState State
        {
            get;
            private set;
        }

        public void Tick()
        {
            if (!instructionMemory.ContainsKey(State.PC))
                throw new NoMoreInstructionsException();
            uint instruction = this.instructionMemory[State.PC];
            byte opCode = (byte)(instruction >> 12);
            ushort args = (ushort)(0XFFF & instruction);
            ops.Get(opCode).Do(State, args);
        }
    }
}
