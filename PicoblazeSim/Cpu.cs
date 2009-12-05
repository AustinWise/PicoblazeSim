using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Austin.PicoblazeSim
{

    /// <summary>
    /// Represents a PicoBlaze processor.
    /// </summary>
    public class Cpu
    {
        /// <summary>
        /// Creates a new instance of the CPU with the given instruction memory.
        /// </summary>
        /// <param name="iMem">instruction memory, mapping memory location to instruction</param>
        public Cpu(Dictionary<ushort, uint> iMem)
        {
            this.instructionMemory = iMem;
            this.state = new CpuState();
        }

        private object exeSync = new object();
        private Dictionary<ushort, uint> instructionMemory;
        private InstructionFactory ops = new InstructionFactory();
        private bool isRunning = false;
        private CpuState state;

        public void RegisterInput(byte id, Func<byte> device)
        {
            state.InputDevices.Add(id, device);
        }

        public void RegisterOutput(byte id, Action<byte> device)
        {
            state.OutputDevices.Add(id, device);
        }

        /// <summary>
        /// States the CPU precessing insructions as fast as it can on a seperate thread.
        /// </summary>
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

        /// <summary>
        /// Executes one instruction.
        /// </summary>
        public void Tick()
        {
            lock (exeSync)
            {
                if (!instructionMemory.ContainsKey(state.ProgramCounter))
                    throw new NoMoreInstructionsException();
                uint instruction = this.instructionMemory[state.ProgramCounter];
                byte opCode = (byte)(instruction >> 12);
                ushort args = (ushort)(0XFFF & instruction);
                ops.Get(opCode).Do(state, args);
            }
        }

        /// <summary>
        /// Signals an interrupt.
        /// </summary>
        public void Interrupt()
        {
            lock (exeSync)
            {
                if (!this.state.EnableInterrupts)
                    return;
                this.state.EnableInterrupts = false;
                this.state.InterruptedProgramCounter = this.state.ProgramCounter;
                this.state.ProgramCounter = 0x3ff;
            }
        }

        /// <summary>
        /// Stops and resets the processor.  
        /// </summary>
        public void Reset()
        {
            lock (exeSync)
            {
                this.isRunning = false;
                this.state = new CpuState();
            }
        }
    }
}
