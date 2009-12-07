using System;
using System.Collections.Generic;
using System.Text;

namespace Austin.PicoblazeSim
{
    /// <summary>
    /// Represents an operation in the CPU.
    /// </summary>
    internal abstract class Operation
    {
        /// <summary>
        /// Executes the operation on the given CPU state with the given arguments.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="args"></param>
        public abstract void Do(CpuState state, ushort args);

        /// <summary>
        /// The type of the first argument to this operation.
        /// </summary>
        public virtual ArgumentType Arg1
        {
            get
            {
                return ArgumentType.None;
            }
        }

        /// <summary>
        /// The type of the second argument to this operation.
        /// </summary>
        public virtual ArgumentType Arg2
        {
            get
            {
                return ArgumentType.None;
            }
        }

        /// <summary>
        /// The number of arguments this operation uses.
        /// </summary>
        public int NumberOfArgs
        {
            get
            {
                return (Arg1 == ArgumentType.None ? 0 : 1) + (Arg2 == ArgumentType.None ? 0 : 1);
            }
        }
    }
}
