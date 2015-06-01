using System;

namespace System.Parsing.Writers.Code
{
    public class UnaryWriter<T> : ParserWriter<T>
        where T : System.Parsing.UnaryParser
    {
        public override void WriteContents(System.Parsing.TextParserWriterArgs args, T parser, string name)
        {
			base.WriteContents(args, parser, name);
			if (parser.Inner != null)
			{
				var child = args.Write(parser.Inner);
				args.Output.WriteLine("{0}.Inner = {1};", name, child);
			}
		}
	}
}