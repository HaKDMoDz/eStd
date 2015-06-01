using System;
using System.Parsing.Parsers;
using System.Parsing.Writers.Code;

namespace System.Parsing.Writers.Code
{
	public class ExceptWriter : UnaryWriter<ExceptParser>
	{
        public override void WriteContents(System.Parsing.TextParserWriterArgs args, ExceptParser parser, string name)
        {
			base.WriteContents(args, parser, name);
			if (parser.Except != null)
				args.Output.WriteLine("{0}.Except = {1};", name, args.Write(parser.Except));
		}
	}
}