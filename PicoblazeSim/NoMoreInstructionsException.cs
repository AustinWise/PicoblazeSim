using System;
using System.Collections.Generic;
using System.Text;

namespace Austin.PicoblazeSim
{
    public class NoMoreInstructionsException : Exception
    {
        public NoMoreInstructionsException()
            : base("There are no more instructions to execute.")
        {
        }
    }
}
