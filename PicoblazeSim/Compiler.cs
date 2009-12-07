using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Austin.PicoblazeSim;
using System.Globalization;

namespace Austin.PicoblazeSim
{
    /// <summary>
    /// Compiles text PicoBlaze programs into instructions.  The inscructions should be the same as the KCPSM3.EXE would generate.
    /// </summary>
    public class Compiler
    {
        private readonly List<string> AddressOps = new List<string>(new string[] { "CALL", "JUMP" });

        private InstructionFactory ops = new InstructionFactory();

        public Dictionary<ushort, uint> Compile(TextReader reader)
        {
            var tokens = getTokens(reader);

            var constants = new Dictionary<string, string>();
            var lablels = new Dictionary<string, ushort>();
            ushort iMemPointer = 0;

            var realTokens = new List<string[]>();

            //get constants and lable address
            foreach (var tok in tokens)
            {
                if (tok[0] == "CONSTANT")
                {
                    constants.Add(tok[1], tok[2]);
                }
                else if (tok[0] == "ADDRESS")
                {
                    iMemPointer = ushort.Parse(tok[1], NumberStyles.HexNumber);
                    realTokens.Add(tok);
                }
                else
                {
                    int colonIndex = tok[0].IndexOf(':');
                    if (colonIndex != -1)
                    {
                        lablels.Add(tok[0].Substring(0, colonIndex), iMemPointer);
                        tok[0] = tok[0].Remove(0, colonIndex + 1);
                        if (tok[0].Length == 0)
                            continue;
                    }

                    //fix shifter ops and whatnot
                    if (ops.IsShifterOp(tok[0]))
                    {
                        var newTok = new string[3];
                        newTok[0] = ops.ShifterFakeName;
                        newTok[1] = tok[1];
                        newTok[2] = ops.GetShifterArgs(tok[0]);
                        realTokens.Add(newTok);
                    }
                    else if ((tok[0] == "ENABLE" || tok[0] == "DISABLE") && tok[1] == "INTERRUPT")
                    {
                        tok[1] = tok[0] == "ENABLE" ? "01" : "00";
                        tok[0] = ops.InterruptEnableFakeName;
                        realTokens.Add(tok);
                    }
                    else if ((tok[0] == "ENABLE" || tok[0] == "DISABLE") && tok[1] == "RETURNI")
                    {
                        tok[1] = tok[1] == "ENABLE" ? "01" : "00";
                        realTokens.Add(tok);
                    }
                    else
                    {
                        realTokens.Add(tok);
                    }

                    iMemPointer++;
                }
            }

            Dictionary<ushort, uint> iMem = new Dictionary<ushort, uint>();
            iMemPointer = 0;

            // convert the fixed tokens into inscructions
            foreach (var tok in realTokens)
            {
                if (tok[0] == "ADDRESS")
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
                    //try to insert labels
                    for (int i = 1; i < tok.Length; i++)
                    {
                        if (lablels.ContainsKey(tok[i]))
                        {
                            tok[i] = "0" + lablels[tok[i]].ToString("x");
                            break;
                        }
                    }

                    ArgumentType arg1Type = tok.Length > 1 ? getArgType(tok[0], tok[1]) : ArgumentType.None;
                    ArgumentType arg2Type = tok.Length > 2 ? getArgType(tok[0], tok[2]) : ArgumentType.None;
                    int argCount = (arg1Type == ArgumentType.None ? 0 : 1) + (arg2Type == ArgumentType.None ? 0 : 1);

                    OperationInfo theOp = null;
                    List<OperationInfo> canidateOperations;
                    try
                    {
                        canidateOperations = ops.Get(tok[0]);
                    }
                    catch (KeyNotFoundException ex)
                    {
                        throw new KeyNotFoundException(string.Format("No instruction by the name of '{0}' exists.", tok[0]), ex);
                    }
                    foreach (var canidateOp in canidateOperations)
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
            }

            return iMem;
        }

        private int[] indexOfBlankInstrs(List<string[]> toks)
        {
            List<int> locs = new List<int>();
            for (int i = 0; i < toks.Count; i++)
            {
                if (toks[i][0] == "")
                    locs.Add(i);
            }
            return locs.ToArray();

        }

        private ushort getArg1Value(ArgumentType type, string[] tok)
        {
            if (type == ArgumentType.None)
                return 0;
            string value = tok[1];
            switch (type)
            {
                case ArgumentType.Register:
                    return (ushort)(byte.Parse(value.Remove(0, 1), NumberStyles.HexNumber) << 8);
                case ArgumentType.Constant:
                    throw new NotSupportedException("Arg1 can't be a constant.");
                case ArgumentType.Address:
                    return ushort.Parse(value, NumberStyles.HexNumber);
                case ArgumentType.FlagCondition:
                    return (ushort)((byte)Enum.Parse(typeof(FlowControlCondition), value) << 10);
                case ArgumentType.Bit:
                    return ushort.Parse(value.Replace("ENABLE", "1").Replace("DISABLE", "0"), NumberStyles.HexNumber);
                default:
                    throw new NotSupportedException("Unsupported flag");
            }
        }

        private ushort getArg2Value(ArgumentType type, string[] tok)
        {
            if (type == ArgumentType.None)
                return 0;
            string value = tok[2];
            switch (type)
            {
                case ArgumentType.Register:
                    return (ushort)(byte.Parse(value.Remove(0, 1), NumberStyles.HexNumber) << 4);
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

        private ArgumentType getArgType(string op, string value)
        {
            byte res;
            if (op == ops.InterruptEnableFakeName || op == "RETURNI")
                return ArgumentType.Bit;
            if (value.Length == 1)
                return ArgumentType.FlagCondition;
            if (value.Length == 2 && value[0] == 'N')
                return ArgumentType.FlagCondition;
            if (value.StartsWith("S") && isHexNumber(value.Remove(0, 1)))
                return ArgumentType.Register;
            if (isHexNumber(value))
            {
                if (AddressOps.Contains(op))
                    return ArgumentType.Address;
                return ArgumentType.Constant;
            }
            return ArgumentType.None;
        }

        private bool isHexNumber(string str)
        {
            ushort res;
            return tryParseHexNumber(str, out res);
        }

        private NumberFormatInfo numFormInfo = new NumberFormatInfo();
        private bool tryParseHexNumber(string str, out ushort res)
        {
            return ushort.TryParse(str, NumberStyles.HexNumber, numFormInfo, out res);
        }

        /// <summary>
        /// Gets raw tokens from the file.  Removes comments, puts lables next to the neareast command (without touching another command),
        /// and splits up command names and args.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private List<string[]> getTokens(TextReader reader)
        {
            List<string[]> tokens = new List<string[]>();

            string line;
            string addToFront = string.Empty; // for lables
            while ((line = reader.ReadLine()) != null)
            {
                line = addToFront + line.ToUpper().Trim();
                addToFront = string.Empty;

                if (line.Length == 0)
                    continue;

                //remove comments
                if (line[0] == ';')
                    continue;
                int commentIndex = line.IndexOf(';');
                if (commentIndex != -1)
                    line = line.Remove(commentIndex);

                // if a label is all by itself on a line, move it to in front of an instruction
                if (line.EndsWith(":"))
                {
                    addToFront = line;
                    continue;
                }

                //remove stacked lables
                int colonIndex;
                while ((colonIndex = line.LastIndexOf(':')) != line.IndexOf(':'))
                {
                    int toRemoveCount = line.IndexOf(':') + 1;
                    tokens.Add(new string[] { line.Substring(0, toRemoveCount) });
                    line = line.Remove(0, toRemoveCount);
                }

                //remove space between colon and op
                while (colonIndex != -1 && line[colonIndex + 1] == ' ')
                    line = line.Remove(colonIndex + 1, 1);

                //get instr name
                int instrEndIndex = line.IndexOf(' ');

                // no args
                if (instrEndIndex == -1)
                {
                    tokens.Add(new string[] { line });
                    continue;
                }

                //get the command name
                string instrName = line.Substring(0, instrEndIndex).ToUpper(); ;

                //remove spaces between args
                line = line.Remove(0, instrEndIndex + 1);
                line = line.Replace(" ", string.Empty);

                //split args
                string[] args = line.Split(',');

                //put token together
                string[] token = new string[args.Length + 1];
                token[0] = instrName;
                Array.Copy(args, 0, token, 1, args.Length);

                tokens.Add(token);
            }

            return tokens;
        }
    }
}
