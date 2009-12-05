using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Austin.PicoblazeSim
{
    public class Operations
    {
        private List<OperationInfo> operations = new List<OperationInfo>();
        private Dictionary<byte, Operation> opCodeToOps = new Dictionary<byte, Operation>();
        private Dictionary<string, List<OperationInfo>> opNameToOpInfo = new Dictionary<string, List<OperationInfo>>();
        private List<string> shifterNames = new List<string>(Enum.GetNames(typeof(ShifterOps)));

        public Operations()
        {
            createLogicOps();
            createMathOps();
            createShifterOps();
            createIoOps();
            createFlowControlOps();
            createCompareAndTestOps();

            foreach (var op in operations)
            {
                opCodeToOps.Add(op.OpCode, op.Op);
                if (!opNameToOpInfo.ContainsKey(op.Name))
                    opNameToOpInfo.Add(op.Name, new List<OperationInfo>());
                opNameToOpInfo[op.Name].Add(op);
            }
        }

        public Operation Get(byte opCode)
        {
            return opCodeToOps[opCode];
        }

        public List<OperationInfo> Get(string name)
        {
            return opNameToOpInfo[name];
        }

        #region Shifter
        public bool IsShifterOp(string name)
        {
            return shifterNames.Contains(name);
        }

        public string ShifterFakeName
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
        public string GetShifterArgs(string shifterName)
        {
            return "0" + ((byte)Enum.Parse(typeof(ShifterOps), shifterName)).ToString("x");
        }

        private void createShifterOps()
        {
            no(ShifterFakeName, 0x20, new ImmediateOperation(doShifter));
        }

        private void doShifter(CpuState state, byte reg, byte typeBits)
        {
            bool oldCarry;
            ShifterOps type = (ShifterOps)typeBits;
            byte val = state.Reg[reg];
            switch (type)
            {
                case ShifterOps.RL:
                    state.C = (0x80 & val) != 0;
                    val = (byte)(val << 1);
                    if (state.C)
                        val++;
                    break;
                case ShifterOps.RR:
                    state.C = (0x01 & val) == 1;
                    val = (byte)(val >> 1);
                    val |= (byte)(state.C ? 0x80 : 0x0);
                    break;
                case ShifterOps.SL0:
                    state.C = (0x80 & val) != 0;
                    val = (byte)(val << 1);
                    break;
                case ShifterOps.SL1:
                    state.C = (0x80 & val) != 0;
                    val = (byte)(val << 1);
                    val |= 0x01;
                    break;
                case ShifterOps.SLA:
                    oldCarry = state.C;
                    state.C = (0x80 & val) != 0;
                    val = (byte)(val << 1);
                    val |= (byte)(oldCarry ? 0x01 : 0x00);
                    break;
                case ShifterOps.SLX:
                    bool oldBottomBit = (0x01 & val) == 1;
                    state.C = (0x80 & val) != 0;
                    val = (byte)(val << 1);
                    val |= (byte)(oldBottomBit ? 0x01 : 0x00);
                    break;
                case ShifterOps.SR0:
                    state.C = (0x01 & val) != 0;
                    val = (byte)(val >> 1);
                    break;
                case ShifterOps.SR1:
                    state.C = (0x01 & val) != 0;
                    val = (byte)(val >> 1);
                    val |= 0x80;
                    break;
                case ShifterOps.SRA:
                    oldCarry = state.C;
                    state.C = (0x01 & val) != 0;
                    val = (byte)(val >> 1);
                    val |= (byte)(oldCarry ? 0x80 : 0x00);
                    break;
                case ShifterOps.SRX:
                    bool oldTopBit = (0x80 & val) == 1;
                    state.C = (0x01 & val) != 0;
                    val = (byte)(val >> 1);
                    val |= (byte)(oldTopBit ? 0x80 : 0x00);
                    break;
                default:
                    throw new NotSupportedException("Unsupported shifter operation.");
            }
            state.Reg[reg] = val;
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
            no("COMPARE", 0x14, new ImmediateOperation((state, a, b) => compare(state, state.Reg[a], b)));
            no("COMPARE", 0x15, new RegisterOperation((state, a, b) => compare(state, state.Reg[a], state.Reg[b])));
        }

        private void compare(CpuState state, byte aVal, byte bVal)
        {
            state.Z = aVal == bVal;
            state.C = aVal < bVal;
        }

        private void test(CpuState state, byte aVal, byte bVal)
        {
            state.Z = (aVal & bVal) == 0;
            //state.C = ;
        }
        #endregion

        #region Mem/IO
        private void createIoOps()
        {
            no("LOAD", 0x0, new ImmediateOperation((state, a, b) => state.Reg[a] = b));
            no("LOAD", 0x1, new RegisterOperation((state, a, b) => state.Reg[a] = state.Reg[b]));

            no("STORE", 0x2E, new ImmediateOperation((state, a, b) => state.Mem[b] = state.Reg[a]));
            no("STORE", 0x2F, new RegisterOperation((state, a, b) => state.Mem[state.Reg[b]] = state.Reg[a]));
            no("FETCH", 0x6, new ImmediateOperation((state, a, b) => state.Reg[a] = state.Mem[b]));
            no("FETCH", 0x7, new RegisterOperation((state, a, b) => state.Reg[a] = state.Mem[state.Reg[b]]));

            no("INPUT", 0x5, new RegisterOperation((state, a, b) => state.Reg[a] = state.InputDevices[state.Reg[b]]()));
            no("INPUT", 0x4, new ImmediateOperation((state, a, b) => state.Reg[a] = state.InputDevices[b]()));
            no("OUTPUT", 0x2D, new RegisterOperation((state, a, b) => state.OutputDevices[state.Reg[b]](state.Reg[a])));
            no("OUTPUT", 0x2C, new ImmediateOperation((state, a, b) => state.OutputDevices[b](state.Reg[a])));
        }
        #endregion

        #region Logic
        private void createLogicOps()
        {
            no("AND", 0xA, new ImmediateOperation((state, a, b) => state.Reg[a] = doLogic(state, state.Reg[a] & b)));
            no("AND", 0xB, new RegisterOperation((state, a, b) => state.Reg[a] = doLogic(state, state.Reg[a] & state.Reg[b])));
            no("OR", 0xC, new ImmediateOperation((state, a, b) => state.Reg[a] = doLogic(state, state.Reg[a] | b)));
            no("OR", 0xD, new RegisterOperation((state, a, b) => state.Reg[a] = doLogic(state, state.Reg[a] | state.Reg[b])));
            no("XOR", 0xE, new ImmediateOperation((state, a, b) => state.Reg[a] = doLogic(state, state.Reg[a] ^ b)));
            no("XOR", 0xF, new RegisterOperation((state, a, b) => state.Reg[a] = doLogic(state, state.Reg[a] ^ state.Reg[b])));
        }

        private byte doLogic(CpuState state, int value)
        {
            state.Z = value == 0;
            state.C = false;
            return (byte)value;
        }
        #endregion

        #region Flow Control
        private void createFlowControlOps()
        {
            no("JUMP", 0x34, new AddressOperation((state, addr) => state.PC = addr));
            no("JUMP", 0x35, new FlowControlAddressOperation((state, c, addr) => state.PC = flowControlConditional(state, c) ? addr : ++state.PC));
            no("CALL", 0x30, new AddressOperation((state, addr) => state.PC = call(state, addr)));
            no("CALL", 0x31, new FlowControlAddressOperation((state, c, addr) => state.PC = flowControlConditional(state, c) ? call(state, addr) : ++state.PC));
            no("RETURN", 0x2A, new ZeroParameterOperation((state) => state.PC = state.CallStack.Pop()));
            no("RETURN", 0x2B, new FlowControlOperation((state, c) => state.PC = flowControlConditional(state, c) ? state.CallStack.Pop() : ++state.PC));
        }

        private ushort call(CpuState state, ushort addr)
        {
            state.CallStack.Push(++state.PC);
            return addr;
        }

        private bool flowControlConditional(CpuState state, FlowControlCondition cond)
        {
            switch (cond)
            {
                case FlowControlCondition.Z:
                    return state.Z;
                case FlowControlCondition.NZ:
                    return !state.Z;
                case FlowControlCondition.C:
                    return state.C;
                case FlowControlCondition.NC:
                    return !state.C;
                default:
                    throw new NotSupportedException("Invalid flag.");
            }
        }
        #endregion

        #region Math
        private void createMathOps()
        {
            no("ADD", 0x18, new ImmediateOperation((state, a, b) => state.Reg[a] = add(state, state.Reg[a], b, false)));
            no("ADD", 0x19, new RegisterOperation((state, a, b) => state.Reg[a] = add(state, state.Reg[a], state.Reg[b], false)));
            no("ADDCY", 0x1A, new ImmediateOperation((state, a, b) => state.Reg[a] = add(state, state.Reg[a], b, true)));
            no("ADDCY", 0x1B, new RegisterOperation((state, a, b) => state.Reg[a] = add(state, state.Reg[a], state.Reg[b], true)));

            no("SUB", 0x1C, new ImmediateOperation((state, a, b) => state.Reg[a] = sub(state, state.Reg[a], b, false)));
            no("SUB", 0x1D, new RegisterOperation((state, a, b) => state.Reg[a] = sub(state, state.Reg[a], state.Reg[b], false)));
            no("SUBCY", 0x1E, new ImmediateOperation((state, a, b) => state.Reg[a] = sub(state, state.Reg[a], b, true)));
            no("SUBCY", 0x1F, new RegisterOperation((state, a, b) => state.Reg[a] = sub(state, state.Reg[a], state.Reg[b], true)));
        }

        private static byte add(CpuState state, byte a, int b, bool cy)
        {
            int ret = a + b + (cy && state.C ? 1 : 0);

            state.C = ret > 255;
            state.Z = (ret == 0) || ((ret + (state.C ? 1 : 0)) == 256);

            return (byte)(0xFF & ret);
        }

        private static byte sub(CpuState state, byte a, int b, bool cy)
        {
            int ret = a - b - (cy && state.C ? 1 : 0);

            state.C = ret < 0;
            state.Z = (ret == 0) || (ret == -256);

            return (byte)(0xFF & Math.Abs(ret));
        }
        #endregion

        private void no(String name, byte opCode, Operation op)
        {
            this.operations.Add(new OperationInfo(opCode, name, op));
        }
    }
}
