using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Austin.PicoblazeSim
{
    public class Cpu
    {
        public Cpu(Dictionary<ushort, uint> iMem)
        {
            this.instructionMemory = iMem;
            this.State = new CpuState();
        }

        private object exeSync = new object();
        private Dictionary<ushort, uint> instructionMemory;
        private Operations ops = new Operations();
        private bool isRunning = false;


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

        public void Start()
        {
            lock (exeSync)
            {
                if (isRunning)
                    return;
                isRunning = true;
                ThreadPool.QueueUserWorkItem(new WaitCallback(loop));
            }
        }

        private void loop(object noUsed)
        {
            while (isRunning)
            {
                try
                {
                    Tick();
                }
                catch (NoMoreInstructionsException)
                {
                    isRunning = false;
                }
            }
        }

        public void Tick()
        {
            lock (exeSync)
            {
                if (!instructionMemory.ContainsKey(State.PC))
                    throw new NoMoreInstructionsException();
                uint instruction = this.instructionMemory[State.PC];
                byte opCode = (byte)(instruction >> 12);
                ushort args = (ushort)(0XFFF & instruction);
                ops.Get(opCode).Do(State, args);
            }
        }

        public void Interrupt()
        {
            lock (exeSync)
            {
                if (!this.State.EnableInterrupts)
                    return;
                this.State.EnableInterrupts = false;
                this.State.InterruptedPC = this.State.PC;
                this.State.PC = 0x3ff;
            }
        }
    }
}
