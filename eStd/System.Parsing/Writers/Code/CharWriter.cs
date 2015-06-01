using System;
using System.Linq;
using System.Parsing.Parsers;
using System.Parsing.Writers.Code;

namespace System.Parsing.Writers.Code
{
	public class CharWriter : InverseWriter<CharTerminal>
	{
        public override void WriteContents(System.Parsing.TextParserWriterArgs args, CharTerminal parser, string name)
        {
			base.WriteContents(args, parser, name);
			if (parser.CaseSensitive != null)
				args.Output.WriteLine("{0}.CaseSensitive = {1};", name, parser.CaseSensitive.HasValue ? parser.CaseSensitive.ToString().ToLowerInvariant() : "null");
		}
	}
}