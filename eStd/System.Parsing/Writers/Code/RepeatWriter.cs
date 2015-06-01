using System;
using System.Parsing.Parsers;
using System.Parsing.Writers.Code;

namespace System.Parsing.Writers.Code
{
	public class RepeatWriter : UnaryWriter<RepeatParser>
	{
        public override void WriteObject(System.Parsing.TextParserWriterArgs args, RepeatParser parser, string name)
        {
			base.WriteObject(args, parser, name);
			args.Output.WriteLine("{0}.Minimum = {1};", name, parser.Minimum);
			if (parser.Maximum != Int32.MaxValue)
				args.Output.WriteLine("{0}.Maximum = {1};", name, parser.Maximum);
			if (parser.Until != null)
				args.Output.WriteLine("{0}.Until = {1};", name, args.Write(parser.Until));
			if (parser.Separator != null)
				args.Output.WriteLine("{0}.Separator = {1};", name, args.Write(parser.Separator));
		}
	}
}