using System;
using System.Collections.Generic;
using System.Text;
using Austin.PicoblazeSim.Operations;

namespace Austin.PicoblazeSim
{
    /// <summary>
    /// Contains all the instructions of the CPU, for both runtime and compile time purposes.
    /// </summary>
    internal class InstructionFactory
    {
        private List<OperationInfo> operations = new List<OperationInfo>();
        private Operation[] opCodeToOps;
        private Dictionary<string, List<OperationInfo>> opNameToOpInfo = new Dictionary<string, List<OperationInfo>>();
        private List<string> shifterNames = new List<string>(Enum.GetNames(typeof(ShifterOps)));
        private List<string> specialOps = new List<string>(new string[] { "ENABLE", "DISABLE", "RETURNI" });

        public InstructionFactory()
        {
            createLogicOps();
            createMathOps();
            createShifterOps();
            createIoOps();
            createFlowControlOps();
            createCompareAndTestOps();
            createInterruptOps();

            add("BREAKPOINT", 0x3F, new ZeroParameterOperation((state) => System.Diagnostics.Debugger.Break()));

            opCodeToOps = new Operation[0x100];

            foreach (var op in operations)
            {
                opCodeToOps[op.OpCode] = op.Op;
                if (!opNameToOpInfo.ContainsKey(op.Name))
                    opNameToOpInfo.Add(op.Name, new List<OperationInfo>());
                opNameToOpInfo[op.Name].Add(op);
            }
        }

        /// <summary>
        /// Gets the instruction with the given op code.
        /// </summary>
        /// <param name="opCode"></param>
        /// <returns></returns>
        public Operation Get(byte opCode)
        {
            return opCodeToOps[opCode];
        }

        /// <summary>
        /// Get instructions that have the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<OperationInfo> Get(string name)
        {
            return opNameToOpInfo[name];
        }
        

        #region Interrupt
        // hack for the compiler since "enable interrupt" and "disable interrupt" don't really fit in.
        internal string InterruptEnableFakeName
        {
            get
            {
                return "__INTERRUPT__";
            }
        }

        private void createInterruptOps()
        {
            add(InterruptEnableFakeName, 0x3C, new BitOperation((state, yes) => state.EnableInterrupts = yes));
            add("RETURNI", 0x38, new BitOperation((state, yes) => state.ReturnFromInterrupt(yes)));
        }
        #endregion

        #region Shifter
        internal bool IsShifterOp(string name)
        {
            return shifterNames.Contains(name);
        }

        //hack for the compiler since all the differnt shifter operations have differnt names yet use the same opcode.
        internal string ShifterFakeName
        {
            get
            {
                return "__SHIFTER__";
            }
        }

        /// <summary>
        /// Turns the shifter name into a argument that could be used as a constant.
        /// </summary>
        /// <param name="shifterName"></param>
        /// <returns></returns>
        internal string GetShifterArgs(string shifterName)
        {
            return "0" + ((byte)Enum.Parse(typeof(ShifterOps), shifterName)).ToString("x");
        }

        private void createShifterOps()
        {
            add(ShifterFakeName, 0x20, new ImmediateOperation(doShifter));
        }

        private void doShifter(CpuState state, byte reg, byte typeBits)
        {
            bool oldCarry;
            ShifterOps type = (ShifterOps)typeBits;
            byte val = state.RegisterFile[reg];
            switch (type)
            {
                case ShifterOps.RL:
                    state.CarryFlag = (0x80 & val) != 0;
                    val = (byte)(val << 1);
                    if (state.CarryFlag)
                        val++;
                    break;
                case ShifterOps.RR:
                    state.CarryFlag = (0x01 & val) == 1;
                    val = (byte)(val >> 1);
                    val |= (byte)(state.CarryFlag ? 0x80 : 0x0);
                    break;
                case ShifterOps.SL0:
                    state.CarryFlag = (0x80 & val) != 0;
                    val = (byte)(val << 1);
                    break;
                case ShifterOps.SL1:
                    state.CarryFlag = (0x80 & val) != 0;
                    val = (byte)(val << 1);
                    val |= 0x01;
                    break;
                case ShifterOps.SLA:
                    oldCarry = state.CarryFlag;
                    state.CarryFlag = (0x80 & val) != 0;
                    val = (byte)(val << 1);
                    val |= (byte)(oldCarry ? 0x01 : 0x00);
                    break;
                case ShifterOps.SLX:
                    bool oldBottomBit = (0x01 & val) == 1;
                    state.CarryFlag = (0x80 & val) != 0;
                    val = (byte)(val << 1);
                    val |= (byte)(oldBottomBit ? 0x01 : 0x00);
                    break;
                case ShifterOps.SR0:
                    state.CarryFlag = (0x01 & val) != 0;
                    val = (byte)(val >> 1);
                    break;
                case ShifterOps.SR1:
                    state.CarryFlag = (0x01 & val) != 0;
                    val = (byte)(val >> 1);
                    val |= 0x80;
                    break;
                case ShifterOps.SRA:
                    oldCarry = state.CarryFlag;
                    state.CarryFlag = (0x01 & val) != 0;
                    val = (byte)(val >> 1);
                    val |= (byte)(oldCarry ? 0x80 : 0x00);
                    break;
                case ShifterOps.SRX:
                    bool oldTopBit = (0x80 & val) == 1;
                    state.CarryFlag = (0x01 & val) != 0;
                    val = (byte)(val >> 1);
                    val |= (byte)(oldTopBit ? 0x80 : 0x00);
                    break;
                default:
                    throw new NotSupportedException("Unsupported shifter operation.");
            }
            state.RegisterFile[reg] = val;
        }

        private enum ShifterOps : byte
        {
            RL = 2,
            RR = 12,
            SL0 = 6,
            SL1 = 7,
            SLA = 0,
            SLX = 4,
            SR0 = 14,
            SR1 = 15,
            SRA = 8,
            SRX = 10
        }
        #endregion

        #region Compare and Test
        private void createCompareAndTestOps()
        {
            add("COMPARE", 0x14, new ImmediateOperation((state, a, b) => compare(state, state.RegisterFile[a], b)));
            add("COMPARE", 0x15, new RegisterOperation((state, a, b) => compare(state, state.RegisterFile[a], state.RegisterFile[b])));
            add("TEST", 0x12, new ImmediateOperation((state, a, b) => test(state, state.RegisterFile[a], b)));
            add("TEST", 0x13, new RegisterOperation((state, a, b) => test(state, state.RegisterFile[a], state.RegisterFile[b])));
        }

        private void compare(CpuState state, byte aVal, byte bVal)
        {
            state.ZeroFlag = aVal == bVal;
            state.CarryFlag = aVal < bVal;
        }

        private void test(CpuState state, byte aVal, byte bVal)
        {
            state.ZeroFlag = (aVal & bVal) == 0;

            //odd parity for C flag
            var xor = aVal ^ bVal;
            int count = 0;
            int mask = 0x80;
            while (mask != 0)
            {
                if ((mask & xor) != 0)
                    count++;
                mask = mask >> 1;
            }
            state.CarryFlag = (count % 2) == 0;
        }
        #endregion

        #region Mem/IO
        private void createIoOps()
        {
            add("LOAD", 0x0, new ImmediateOperation((state, a, b) => state.RegisterFile[a] = b));
            add("LOAD", 0x1, new RegisterOperation((state, a, b) => state.RegisterFile[a] = state.RegisterFile[b]));

            add("STORE", 0x2E, new ImmediateOperation((state, a, b) => state.ScratchPadMemory[b] = state.RegisterFile[a]));
            add("STORE", 0x2F, new RegisterOperation((state, a, b) => state.ScratchPadMemory[state.RegisterFile[b]] = state.RegisterFile[a]));
            add("FETCH", 0x6, new ImmediateOperation((state, a, b) => state.RegisterFile[a] = state.ScratchPadMemory[b]));
            add("FETCH", 0x7, new RegisterOperation((state, a, b) => state.RegisterFile[a] = state.ScratchPadMemory[state.RegisterFile[b]]));

            add("INPUT", 0x5, new RegisterOperation((state, a, b) => state.RegisterFile[a] = state.InputDevices[state.RegisterFile[b]]()));
            add("INPUT", 0x4, new ImmediateOperation((state, a, b) => state.RegisterFile[a] = state.InputDevices[b]()));
            add("OUTPUT", 0x2D, new RegisterOperation((state, a, b) => state.OutputDevices[state.RegisterFile[b]](state.RegisterFile[a])));
            add("OUTPUT", 0x2C, new ImmediateOperation((state, a, b) => state.OutputDevices[b](state.RegisterFile[a])));
        }
        #endregion

        #region Logic
        private void createLogicOps()
        {
            add("AND", 0xA, new ImmediateOperation((state, a, b) => state.RegisterFile[a] = doLogic(state, state.RegisterFile[a] & b)));
            add("AND", 0xB, new RegisterOperation((state, a, b) => state.RegisterFile[a] = doLogic(state, state.RegisterFile[a] & state.RegisterFile[b])));
            add("OR", 0xC, new ImmediateOperation((state, a, b) => state.RegisterFile[a] = doLogic(state, state.RegisterFile[a] | b)));
            add("OR", 0xD, new RegisterOperation((state, a, b) => state.RegisterFile[a] = doLogic(state, state.RegisterFile[a] | state.RegisterFile[b])));
            add("XOR", 0xE, new ImmediateOperation((state, a, b) => state.RegisterFile[a] = doLogic(state, state.RegisterFile[a] ^ b)));
            add("XOR", 0xF, new RegisterOperation((state, a, b) => state.RegisterFile[a] = doLogic(state, state.RegisterFile[a] ^ state.RegisterFile[b])));
        }

        private byte doLogic(CpuState state, int value)
        {
            state.ZeroFlag = value == 0;
            state.CarryFlag = false;
            return (byte)value;
        }
        #endregion

        #region Flow Control
        private void createFlowControlOps()
        {
            add("JUMP", 0x34, new AddressOperation((state, addr) => addr));
            add("JUMP", 0x35, new FlowControlAddressOperation((state, c, addr) => state.ProgramCounter = flowControlConditional(state, c) ? addr : ++state.ProgramCounter));
            add("CALL", 0x30, new AddressOperation((state, addr) => call(state, addr)));
            add("CALL", 0x31, new FlowControlAddressOperation((state, c, addr) => state.ProgramCounter = flowControlConditional(state, c) ? call(state, addr) : ++state.ProgramCounter));
            add("RETURN", 0x2A, new ZeroParameterOperation((state) => state.ProgramCounter = state.CallStack.Pop()));
            add("RETURN", 0x2B, new FlowControlOperation((state, c) => state.ProgramCounter = flowControlConditional(state, c) ? state.CallStack.Pop() : ++state.ProgramCounter));
        }

        private ushort call(CpuState state, ushort addr)
        {
            state.CallStack.Push(++state.ProgramCounter);
            return addr;
        }

        private bool flowControlConditional(CpuState state, FlowControlCondition cond)
        {
            switch (cond)
            {
                case FlowControlCondition.Z:
                    return state.ZeroFlag;
                case FlowControlCondition.NZ:
                    return !state.ZeroFlag;
                case FlowControlCondition.C:
                    return state.CarryFlag;
                case FlowControlCondition.NC:
                    return !state.CarryFlag;
                default:
                    throw new NotSupportedException("Invalid flag.");
            }
        }
        #endregion

        #region Math
        private void createMathOps()
        {
            add("ADD", 0x18, new ImmediateOperation((state, a, b) => state.RegisterFile[a] = add(state, state.RegisterFile[a], b, false)));
            add("ADD", 0x19, new RegisterOperation((state, a, b) => state.RegisterFile[a] = add(state, state.RegisterFile[a], state.RegisterFile[b], false)));
            add("ADDCY", 0x1A, new ImmediateOperation((state, a, b) => state.RegisterFile[a] = add(state, state.RegisterFile[a], b, true)));
            add("ADDCY", 0x1B, new RegisterOperation((state, a, b) => state.RegisterFile[a] = add(state, state.RegisterFile[a], state.RegisterFile[b], true)));

            add("SUB", 0x1C, new ImmediateOperation((state, a, b) => state.RegisterFile[a] = sub(state, state.RegisterFile[a], b, false)));
            add("SUB", 0x1D, new RegisterOperation((state, a, b) => state.RegisterFile[a] = sub(state, state.RegisterFile[a], state.RegisterFile[b], false)));
            add("SUBCY", 0x1E, new ImmediateOperation((state, a, b) => state.RegisterFile[a] = sub(state, state.RegisterFile[a], b, true)));
            add("SUBCY", 0x1F, new RegisterOperation((state, a, b) => state.RegisterFile[a] = sub(state, state.RegisterFile[a], state.RegisterFile[b], true)));
        }

        private static byte add(CpuState state, byte a, int b, bool cy)
        {
            int ret = a + b + (cy && state.CarryFlag ? 1 : 0);

            state.CarryFlag = ret > 255;
            state.ZeroFlag = (ret == 0) || ((ret + (state.CarryFlag ? 1 : 0)) == 256);

            return (byte)(0xFF & ret);
        }

        private static byte sub(CpuState state, byte a, int b, bool cy)
        {
            int ret = a - b - (cy && state.CarryFlag ? 1 : 0);

            state.CarryFlag = ret < 0;
            state.ZeroFlag = (ret == 0) || (ret == -256);

            return (byte)(0xFF & ret);
        }
        #endregion

        private void add(String name, byte opCode, Operation op)
        {
            this.operations.Add(new OperationInfo(opCode, name, op));
        }
    }
}
