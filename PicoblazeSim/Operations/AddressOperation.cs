using System;
using System.Collections.Generic;
using System.Text;

namespace Austin.PicoblazeSim.Operations
{
    internal class AddressOperation : Operation
    {
        public AddressOperation(Func<CpuState, ushort, ushort> func)
        {
            this.func = func;
        }

        private Func<CpuState, ushort, ushort> func;

        public override void Do(CpuState state, ushort args)
        {
            state.ProgramCounter = func(state, (ushort)args);
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
