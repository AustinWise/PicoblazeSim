using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Austin.PicoblazeSim;
using System.Globalization;

namespace Austin.PicoblazeCompile
{
    public static class Compiler
    {
        public static Dictionary<ushort, uint> Compile(TextReader reader)
        {
            Operations ops = new Operations();

            var tokens = getTokens(reader);

            var constants = new Dictionary<string, string>();

            Dictionary<ushort, uint> iMem = new Dictionary<ushort, uint>();
            ushort iMemPointer = 0;

            foreach (var tok in tokens)
            {
                if (tok[0] == "CONSTANT")
                {
                    constants.Add(tok[1], tok[2]);
                }
                else if (tok[0] == "ADDRESS")
                {
                    iMemPointer = ushort.Parse(tok[1], NumberStyles.HexNumber);
                }
                else
                {
                    //try to insert constants
                    for (int i = 1; i < tok.Length; i++)
                    {
                        if (constants.ContainsKey(tok[i]))
                        {
                            tok[i] = constants[tok[i]];
                            break;
                        }
                    }
                }

                ArgumentType arg1Type = tok.Length > 1 ? getArgType(tok[1]) : ArgumentType.None;
                ArgumentType arg2Type = tok.Length > 2 ? getArgType(tok[2]) : ArgumentType.None;
                int argCount = (arg1Type == ArgumentType.None ? 0 : 1) + (arg2Type == ArgumentType.None ? 0 : 1);

                OperationInfo theOp = null;
                foreach (var canidateOp in ops.Get(tok[0]))
                {
                    if (canidateOp.Op.NumberOfArgs == argCount)
                    {
                        if ((canidateOp.Op.Arg1 == arg1Type) && (canidateOp.Op.Arg2 == arg2Type))
                        {
                            theOp = canidateOp;
                            break;
                        }
                    }
                }

                if (theOp == null)
                    throw new Exception(String.Format("Could not find a matching op for '{0} {1}'.", tok[0], String.Join(",", tok, 1, tok.Length - 1)));

                ushort args = 0;
                args |= getArg1Value(arg1Type, tok);
                args |= getArg2Value(arg2Type, tok);

                uint instr = (uint)((theOp.OpCode << 12) | args);

                iMem.Add(iMemPointer, instr);
                iMemPointer++;
            }

            return iMem;
        }

        private static ushort getArg1Value(ArgumentType type, string[] tok)
        {
            if (type == ArgumentType.None)
                return 0;
            string value = tok[1];
            switch (type)
            {
                case ArgumentType.Register:
                    return (ushort)(byte.Parse(value.Remove(0, 1)) << 8);
                case ArgumentType.Constant:
                    throw new NotSupportedException("Arg1 can't be a constant.");
                case ArgumentType.Address:
                    return ushort.Parse(value, NumberStyles.HexNumber);
                case ArgumentType.FlagCondition:
                    return (ushort)((byte)Enum.Parse(typeof(FlowControlCondition), value) << 10);
                case ArgumentType.Bit:
                    throw new NotImplementedException();
                default:
                    throw new NotSupportedException("Unsupported flag");
            }
        }

        private static ushort getArg2Value(ArgumentType type, string[] tok)
        {
            if (type == ArgumentType.None)
                return 0;
            string value = tok[2];
            switch (type)
            {
                case ArgumentType.Register:
                    return (ushort)(byte.Parse(value.Remove(0, 1)) << 4);
                case ArgumentType.Constant:
                    return byte.Parse(value, NumberStyles.HexNumber);
                case ArgumentType.Address:
                    return ushort.Parse(value, NumberStyles.HexNumber);
                case ArgumentType.FlagCondition:
                    throw new NotSupportedException("Arg2 can't be a flow control condition.");
                case ArgumentType.Bit:
                    throw new NotSupportedException("Arg2 can't be a bit.");
                default:
                    throw new NotSupportedException("Unsupported flag");
            }
        }

        private static ArgumentType getArgType(string value)
        {
            byte res;
            if (value.Length == 1)
                return ArgumentType.FlagCondition;
            if (value.StartsWith("s") && isHexNumber(value.Remove(0, 1)))
                return ArgumentType.Register;
            if (isHexNumber(value))
                return ArgumentType.Constant;
            return ArgumentType.None;
        }

        private static bool isHexNumber(string str)
        {
            byte res;
            return tryParseHexNumber(str, out res);
        }

        private static NumberFormatInfo numFormInfo = new NumberFormatInfo();
        private static bool tryParseHexNumber(string str, out byte res)
        {
            return byte.TryParse(str, NumberStyles.HexNumber, numFormInfo, out res);
        }

        private static List<string[]> getTokens(TextReader reader)
        {
            List<string[]> tokens = new List<string[]>();

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                line = line.Trim();
                if (line[0] == ';')
                    continue;
                int commentIndex = line.IndexOf(';');
                if (commentIndex != -1)
                    line = line.Remove(commentIndex);

                //get instr name
                int instrEndIndex = line.IndexOf(' ');

                // no args
                if (instrEndIndex == -1)
                {
                    tokens.Add(new string[] { line.ToUpper() });
                    continue;
                }

                string instrName = line.Substring(0, instrEndIndex).ToUpper(); ;

                //remove spaces between args
                line = line.Remove(0, instrEndIndex + 1);
                line = line.Replace(" ", string.Empty);

                string[] args = line.Split(',');

                string[] token = new string[args.Length + 1];
                token[0] = instrName;
                Array.Copy(args, 0, token, 1, args.Length);

                tokens.Add(token);
            }

            return tokens;
        }
    }
}
