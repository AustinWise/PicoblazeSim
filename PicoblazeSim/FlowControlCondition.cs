using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Austin.PicoblazeSim
{
    public enum FlowControlCondition : byte
    {
        Z = 0,
        NZ = 1,
        C = 2,
        NC = 3
    }
}
