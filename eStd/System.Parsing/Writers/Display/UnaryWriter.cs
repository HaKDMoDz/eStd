using System;

namespace System.Parsing.Writers.Display
{
    public class UnaryWriter<T> : ParserWriter<T>
        where T : System.Parsing.UnaryParser
    {
        public override void WriteContents(System.Parsing.TextParserWriterArgs args, T parser, string name)
        {
			args.Write(parser.Inner);
		}
	}
}