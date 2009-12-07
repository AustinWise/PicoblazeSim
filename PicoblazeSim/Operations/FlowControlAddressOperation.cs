using System;
using System.Collections.Generic;
using System.Text;

namespace Austin.PicoblazeSim.Operations
{
    internal class FlowControlAddressOperation : Operation
    {
        public FlowControlAddressOperation(Func<CpuState, FlowControlCondition, ushort, ushort> func)
        {
            this.func = func;
        }

        private Func<CpuState, FlowControlCondition, ushort, ushort> func;

        public override void Do(CpuState state, ushort args)
        {
            state.ProgramCounter = func(state, (FlowControlCondition)(args >> 10), (ushort)(0x3FF & args));
        }

        public override ArgumentType Arg1
        {
            get
            {
                return ArgumentType.FlagCondition;
            }
        }

        public override ArgumentType Arg2
        {
            get
            {
                return ArgumentType.Address;
            }
        }
    }
}
