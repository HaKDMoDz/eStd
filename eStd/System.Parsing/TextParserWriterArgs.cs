using System;
using System.CodeDom.Compiler;
using System.Parsing;

namespace System.Parsing
{
	public class TextParserWriterArgs : ParserWriterArgs
	{
		public IndentedTextWriter Output { get; internal set; }

		public override int Level
		{
			get { return Output.Indent; }
			set { Output.Indent = value; }
		}

		public new TextParserWriter Writer
		{ 
			get { return (TextParserWriter)base.Writer; }
			set { base.Writer = value; } 
		}
	}
}