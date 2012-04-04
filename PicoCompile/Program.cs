using System;
using System.Collections.Generic;
using System.Text;
using Austin.PicoblazeSim;
using System.IO;

namespace Austin.PicoCompile
{
    /// <summary>
    /// A clone of KCPSM3.EXE.
    /// </summary>
    class Program
    {
        //number of instruction slots
        private const ushort PROGRAM_SIZE = 0x400;
        private const int INSTRUCTION_SIZE = 18;

        private static string Name;
        private static string OutputFolder;
        private static uint[] Rom;

        static int Main(string[] args)
        {
            Console.WriteLine("PicoCompile - http://github.com/AustinWise/PicoblazeSim");
            Console.WriteLine("(c) 2009 Austin Wise");
            Console.WriteLine();

            if (args.Length != 1)
            {
                usage();
                return -1;
            }

            string pathOfPsmFile = args[0];
            var fi = new FileInfo(pathOfPsmFile);

            if (!fi.Exists)
            {
                Console.WriteLine("{0} does not exist.", pathOfPsmFile);
                usage();
                return -1;
            }

            OutputFolder = fi.DirectoryName;
            Name = fi.Name;
            Name = Name.Substring(0, Name.LastIndexOf('.'));

            try
            {
                Rom = compile(pathOfPsmFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to compile: {0}", ex.Message);
                return -1;
            }


            Console.WriteLine("Writing coefficient file");
            writeCoe();

            Console.WriteLine("Writing VHDL memory definition file");
            writeVhd();

            Console.WriteLine("Writing Verilog memory definition file");
            writeV();

            Console.WriteLine("Writing System Generator memory definition file");
            writeM();

            Console.WriteLine("Writing memory definition files");
            writeHex(false, ".hex");
            writeDec();
            writeHex(true, ".mem");

            Console.WriteLine();
            Console.WriteLine("PicoCompile completed successfully.");

            return 0;
        }

        private static void usage()
        {
            Console.WriteLine("Usage: PicoCompile.exe <some_prog.psm>");
        }

        #region HDL
        private static void writeVhd()
        {
            string file = Properties.Resources.ROM_form_vhd;
            string ext = ".vhd";

            file = writeHdl(file, ext);
        }

        private static void writeV()
        {
            string file = Properties.Resources.ROM_form_v;
            string ext = ".v";

            file = writeHdl(file, ext);
        }

        private static string writeHdl(string file, string ext)
        {
            string beginTemplate = "{begin template}\r\n";
            file = file.Remove(0, file.IndexOf(beginTemplate) + beginTemplate.Length);

            // subsitute things in
            file = file.Replace("{name}", Name);
            file = file.Replace("{timestamp}", DateTime.Now.ToString("ddMMMyyyy-HH:mm:ss"));

            foreach (var kvp in getMap())
            {
                file = file.Replace('{' + kvp.Key + '}', kvp.Value);
            }

            File.WriteAllText(Path.Combine(OutputFolder, Name + ext), file);
            return file;
        }

        private static Dictionary<string, string> getMap()
        {
            var dic = new Dictionary<string, string>();

            //get top two bits of each instr
            for (int i = 0; i < 0x8; i++)
            {
                string value = "";
                uint num = 0;
                int bits = 0;
                for (int j = 0x7F; j >= 0; j--)
                {
                    num = num << 2;
                    num = num | (Rom[(i << 7) | j] >> 16);
                    bits += 2;
                    if (bits == 8)
                    {
                        value += string.Format("{0,2:x}", num);
                        num = 0;
                        bits = 0;
                    }
                }
                dic.Add(upAndZero(string.Format("INITP_{0,2:x}", i)), upAndZero(value));
            }

            //get bottom two bytes of each instr
            for (int i = 0; i < 0x40; i++)
            {
                string value = "";
                for (int j = 0xF; j >= 0; j--)
                {
                    value += formatWord(Rom[(i << 4) | j]).Remove(0, 1);
                }
                dic.Add(upAndZero(string.Format("INIT_{0,2:x}", i)), value);
            }

            return dic;
        }
        #endregion

        #region Not HDL
        private static void writeCoe()
        {
            string file = Properties.Resources.ROM_form_coe;
            file = file.Replace("{name}", Name);

            StringBuilder sb = new StringBuilder(file);
            for (int i = 0; i < PROGRAM_SIZE; i++)
            {
                sb.AppendFormat("{0}, ", formatWord(Rom[i]));
                if ((i & 0xF) == 0xF)
                {
                    sb.Remove(sb.Length - 1, 1);
                    sb.AppendLine();
                }
            }
            int charactersToRemove = Environment.NewLine.Length + 1;
            sb.Remove(sb.Length - charactersToRemove, charactersToRemove);
            sb.Append(";");
            sb.AppendLine();
            File.WriteAllText(Path.Combine(OutputFolder, Name + ".coe"), sb.ToString());
        }

        private static void writeM()
        {
            StreamWriter sw = new StreamWriter(Path.Combine(OutputFolder, Name + ".m"));
            sw.WriteLine("function bits = fill_prog_rom_program_store()");
            sw.WriteLine("  bits = [ ...");

            for (int topSix = 0; topSix < 0x40; topSix++)
            {
                sw.Write("    ");
                for (int bottomFour = 0; bottomFour < 0x10; bottomFour++)
                {
                    sw.Write("{0}, ", Rom[(topSix << 4) | bottomFour]);
                }
                sw.WriteLine("...");
            }

            sw.WriteLine("  ];");
            sw.WriteLine();
            sw.WriteLine("  return;");

            sw.Close();
        }

        private static void writeDec()
        {
            StreamWriter sw = new StreamWriter(Path.Combine(OutputFolder, Name + ".dec"));
            foreach (var value in Rom)
            {
                sw.WriteLine(" {0} ", value);
            }
            sw.Close();
        }

        private static void writeHex(bool includeStartAddress, string ext)
        {
            StreamWriter sw = new StreamWriter(Path.Combine(OutputFolder, Name + ext));
            if (includeStartAddress)
                sw.WriteLine("@00000000");
            foreach (var value in Rom)
            {
                sw.WriteLine(upAndZero(string.Format("{0,5:x}", value)));
            }
            sw.Close();
        }
        #endregion

        private static uint[] compile(string path)
        {
            var comp = new Compiler();
            var prog = comp.Compile(new StringReader(File.ReadAllText(path)));
            var rom = new uint[PROGRAM_SIZE];

            for (ushort i = 0; i < PROGRAM_SIZE; i++)
            {
                uint value = 0;
                if (prog.ContainsKey(i))
                    value = prog[i];
                rom[i] = value;
            }
            return rom;
        }

        private static string formatWord(uint word)
        {
            return upAndZero(string.Format("{0,5:x}", word));
        }

        private static string upAndZero(string spacesAndLower)
        {
            return spacesAndLower.Replace(' ', '0').ToUpperInvariant();
        }
    }
}
