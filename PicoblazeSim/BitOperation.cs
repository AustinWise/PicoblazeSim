using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Austin.PicoblazeSim
{
    public class BitOperation : Operation
    {
        public BitOperation(Func<CpuState, bool, ushort> func)
        {
            this.func = func;
        }

        private Func<CpuState, bool, ushort> func;

        public override void Do(CpuState state, ushort args)
        {
            state.PC = func(state, args != 0);
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
