using System;
using System.Parsing.Parsers;
using System.Parsing.Writers.Code;
using System.Linq;

namespace System.Parsing.Writers.Code
{
	public class CharSetWriter : InverseWriter<CharSetTerminal>
	{
        public override void WriteContents(System.Parsing.TextParserWriterArgs args, CharSetTerminal tester, string name)
        {
			base.WriteContents(args, tester, name);
			args.Output.WriteLine("{0}.Characters = new char[] {{ {1} }}; // {2}", 
			                      name, 
			                      string.Join(", ", tester.Characters.Select(r => string.Format("(char)0x{0:x}", (int)r))),
			                      new string(tester.Characters));
		}
	}
}