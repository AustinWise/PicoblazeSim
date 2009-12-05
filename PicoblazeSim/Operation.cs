using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Austin.PicoblazeSim
{
    public abstract class Operation
    {
        public abstract void Do(CpuState state, ushort args);

        public virtual ArgumentType Arg1
        {
            get
            {
                return ArgumentType.None;
            }
        }

        public virtual ArgumentType Arg2
        {
            get
            {
                return ArgumentType.None;
            }
        }

        public int NumberOfArgs
        {
            get
            {
                return (Arg1 == ArgumentType.None ? 0 : 1) + (Arg2 == ArgumentType.None ? 0 : 1);
            }
        }
    }
}
