using System;
using System.Collections.Generic;
using System.Text;

namespace Austin.PicoblazeSim
{
    internal enum ArgumentType
    {
        None,
        Register,
        Constant,
        Address,
        FlagCondition,
        Bit
    }
}
