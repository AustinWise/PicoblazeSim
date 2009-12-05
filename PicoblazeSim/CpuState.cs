using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Austin.PicoblazeSim
{
    public class CpuState
    {
        public CpuState()
        {
            this.CallStack = new Stack<ushort>();
            this.InputDevices = new Dictionary<byte, Func<byte>>();
            this.OutputDevices = new Dictionary<byte, Action<byte>>();
            this.Reg = new byte[16];
            this.Mem = new byte[64];
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

        bool InterruptEnabled
        {
            get;
            set;
        }

        public bool Z
        {
            get;
            set;
        }

        public bool C
        {
            get;
            set;
        }

        public byte[] Reg
        {
            get;
            private set;
        }

        public byte[] Mem
        {
            get;
            private set;
        }

        public ushort PC
        {
            get;
            set;
        }

        public Stack<ushort> CallStack
        {
            get;
            private set;
        }
    }
}
