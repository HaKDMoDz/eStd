using System;
using System.Parsing.Parsers;
using System.Parsing.Writers.Display;

namespace System.Parsing.Writers.Display
{
	public class RepeatWriter : UnaryWriter<RepeatParser>
	{
        public override string GetName(System.Parsing.ParserWriterArgs args, RepeatParser parser)
        {
			if (parser.Maximum == Int32.MaxValue)
				return string.Format("{0} [Min: {1}]", base.GetName(args, parser), parser.Minimum);
			else
				return string.Format("{0} [Min: {1}, Max: {2}]", base.GetName(args, parser), parser.Minimum, parser.Maximum);
		}
	}
}