﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Austin.PicoblazeSim
{
    public class BitOperation : Operation
    {
        public BitOperation(Action<CpuState, bool> func)
        {
            this.func = func;
        }

        private Action<CpuState, bool> func;

        public override void Do(CpuState state, ushort args)
        {
            state.PC++;
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
