using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Austin.PicoblazeSim
{
    public class AddressOperation : Operation
    {
        public AddressOperation(Func<CpuState, ushort, ushort> func)
        {
            this.func = func;
        }

        private Func<CpuState, ushort, ushort> func;

        public override void Do(CpuState state, ushort args)
        {
            state.PC = func(state, (ushort)args);
        }

        public override ArgumentType Arg1
        {
            get
            {
                return ArgumentType.Address;
            }
        }
    }
}
