using System;
using System.Collections.Generic;
using System.Text;

namespace Austin.PicoblazeSim
{
    /// <summary>
    /// Represents the state of the CPU.  Some limitations of the VHDL PicoBlaze processor are not enforced.
    /// </summary>
    internal class CpuState
    {
        public CpuState()
        {
            this.CallStack = new Stack<ushort>();
            this.InputDevices = new Dictionary<byte, Func<byte>>();
            this.OutputDevices = new Dictionary<byte, Action<byte>>();
            this.RegisterFile = new byte[16];
            this.ScratchPadMemory = new byte[64];
            this.EnableInterrupts = false;
            this.ProgramCounter = 0;
        }

        public Dictionary<byte, Func<byte>> InputDevices
        {
            get;
            private set;
        }

        public Dictionary<byte, Action<byte>> OutputDevices
        {
            get;
            private set;
        }

        public bool ZeroFlag
        {
            get;
            set;
        }

        public bool CarryFlag
        {
            get;
            set;
        }

        /// <summary>
        /// 16 bytes, limited
        /// </summary>
        public byte[] RegisterFile
        {
            get;
            private set;
        }

        /// <summary>
        /// 64 bytes, limited
        /// </summary>
        public byte[] ScratchPadMemory
        {
            get;
            private set;
        }

        /// <summary>
        /// 10 bits, not limited
        /// </summary>
        public ushort ProgramCounter
        {
            get;
            set;
        }

        /// <summary>
        /// 31 deep, not limited
        /// </summary>
        public Stack<ushort> CallStack
        {
            get;
            private set;
        }

        /// <summary>
        /// Indicates whether interrupts should be processed.
        /// </summary>
        public bool EnableInterrupts
        {
            get;
            set;
        }

        /// <summary>
        /// The location to execute upon returning from the interrupt.
        /// </summary>
        public ushort InterruptedProgramCounter
        {
            get;
            set;
        }

        /// <summary>
        /// Returns from the interrupt.
        /// </summary>
        /// <param name="reenable"></param>
        public void ReturnFromInterrupt(bool reenable)
        {
            this.ProgramCounter = InterruptedProgramCounter;
            this.EnableInterrupts = reenable;
        }
    }
}
