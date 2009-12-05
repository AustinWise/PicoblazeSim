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
            this.Reg = new byte[16];
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
