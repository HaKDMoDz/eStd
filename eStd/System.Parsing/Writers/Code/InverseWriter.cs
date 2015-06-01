using System;
using System.Parsing;
using System.Linq;

namespace System.Parsing.Writers.Code
{
	public class InverseWriter<T> : ParserWriter<T>
		where T: Parser, IInverseParser
	{
		public override void WriteContents(TextParserWriterArgs args, T parser, string name)
		{
			base.WriteContents(args, parser, name);
			if (parser.Inverse)
				args.Output.WriteLine("{0}.Inverse = {1};", name, parser.Inverse.ToString().ToLower());
		}
	}
}