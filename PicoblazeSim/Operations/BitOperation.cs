using System;
using System.Collections.Generic;
using System.Text;

namespace Austin.PicoblazeSim.Operations
{
    internal class BitOperation : Operation
    {
        public BitOperation(Action<CpuState, bool> func)
        {
            this.func = func;
        }

        private Action<CpuState, bool> func;

        public override void Do(CpuState state, ushort args)
        {
            state.ProgramCounter++;
            func(state, args != 0);
        }

        public override ArgumentType Arg1
        {
            get
            {
                return ArgumentType.Bit;
            }
        }
    }
}
