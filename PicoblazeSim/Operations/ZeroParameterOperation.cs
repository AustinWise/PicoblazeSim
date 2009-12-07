using System;
using System.Collections.Generic;
using System.Text;

namespace Austin.PicoblazeSim.Operations
{
    internal class ZeroParameterOperation : Operation
    {
        public ZeroParameterOperation(Action<CpuState> action)
        {
            this.action = action;
        }

        private Action<CpuState> action;

        public override void Do(CpuState state, ushort args)
        {
            state.ProgramCounter++;
            action(state);
        }
    }
}
