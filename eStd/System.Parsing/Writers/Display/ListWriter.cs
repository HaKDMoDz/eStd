using System;
using System.Parsing;
using System.Parsing.Writers.Display;

namespace System.Parsing.Writers.Display
{
	public class ListWriter : ParserWriter<ListParser>
	{
		public override void WriteContents(TextParserWriterArgs args, ListParser parser, string name)
		{
			parser.Items.ForEach(r => args.Write(r));
		}
	}
}