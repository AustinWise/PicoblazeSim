using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Austin.PicoblazeSim
{
    public class FlowControlOperation : Operation
    {
        public FlowControlOperation(Func<CpuState, FlowControlCondition, ushort, ushort> func)
        {
            this.func = func;
        }

        private Func<CpuState, FlowControlCondition, ushort, ushort> func;

        public override void Do(CpuState state, ushort args)
        {
            state.PC = func(state, (FlowControlCondition)(args >> 10), (ushort)(0x3FF & args));
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
