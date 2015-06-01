using System;
using System.Parsing.Parsers;

namespace System.Parsing.Writers.Code
{
	public class LiteralWriter : ParserWriter<LiteralTerminal>
	{
        public override void WriteObject(System.Parsing.TextParserWriterArgs args, LiteralTerminal parser, string name)
        {
			base.WriteObject(args, parser, name);
			if (parser.CaseSensitive != null)
				args.Output.WriteLine("{0}.CaseSensitive = {1};", name, parser.CaseSensitive.ToString().ToLowerInvariant());
			if (parser.Value != null)
				args.Output.WriteLine("{0}.Value = \"{1}\";", name, parser.Value.Replace("\"", "\\\""));
		}
	}
	
}