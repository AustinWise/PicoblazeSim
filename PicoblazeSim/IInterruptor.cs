using System;
using System.Collections.Generic;
using System.Text;

namespace Austin.PicoblazeSim
{
    /// <summary>
    /// Represents something that can interrupt the CPU.
    /// </summary>
    public interface IInterruptor
    {
        event EventHandler Interrupt;
    }
}
