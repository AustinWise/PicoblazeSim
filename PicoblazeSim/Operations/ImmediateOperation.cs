using System;
using System.Collections.Generic;
using System.Text;

namespace Austin.PicoblazeSim.Operations
{
    internal class ImmediateOperation : Operation
    {
        public ImmediateOperation(Action<CpuState, byte, byte> action)
        {
            this.action = action;
        }

        private Action<CpuState, byte, byte> action;

        public override void Do(CpuState state, ushort args)
        {
            state.ProgramCounter++;
            action(state, (byte)(args >> 8), (byte)(0xFF & args));
        }

        public override ArgumentType Arg1
        {
            get
            {
                return ArgumentType.Register;
            }
        }

        public override ArgumentType Arg2
        {
            get
            {
                return ArgumentType.Constant;
            }
        }
    }
}
