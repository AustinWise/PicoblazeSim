using System;
using System.Collections.Generic;
using System.Text;

namespace Austin.PicoblazeSim
{
    internal class OperationInfo
    {
        public OperationInfo(byte opCode, string name, Operation op)
        {
            this.OpCode = opCode;
            this.Op = op;
            this.Name = name;
        }

        public byte OpCode { get; private set; }
        public string Name { get; private set; }
        public Operation Op { get; private set; }
    }
}
