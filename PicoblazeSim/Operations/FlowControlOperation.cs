using System;
using System.Collections.Generic;
using System.Text;

namespace Austin.PicoblazeSim.Operations
{
    internal class FlowControlOperation : Operation
    {
        public FlowControlOperation(Func<CpuState, FlowControlCondition, ushort> func)
        {
            this.func = func;
        }

        private Func<CpuState, FlowControlCondition, ushort> func;

        public override void Do(CpuState state, ushort args)
        {
            state.ProgramCounter = func(state, (FlowControlCondition)(args >> 10));
        }

        public override ArgumentType Arg1
        {
            get
            {
                return ArgumentType.FlagCondition;
            }
        }
    }
}
