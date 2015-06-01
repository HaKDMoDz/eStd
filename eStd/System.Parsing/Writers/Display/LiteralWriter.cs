using System;
using System.Parsing.Parsers;
using System.Parsing.Writers.Display;

namespace System.Parsing.Writers.Display
{
	public class LiteralWriter : ParserWriter<LiteralTerminal>
	{
        public override string GetName(System.Parsing.ParserWriterArgs args, LiteralTerminal parser)
        {
			return string.Format("{0} [Value: '{1}']", base.GetName(args, parser), parser.Value);
		}
	}
}