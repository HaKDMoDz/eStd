using System;
using System.Parsing;

namespace System.Parsing
{
	/// <summary>
	/// Arguments to determine if a parser is contained within another
	/// </summary>
	public class ParserContainsArgs : ParserChain
	{
		public Parser Parser { get; private set; }

		public ParserContainsArgs(Parser parser)
		{
			this.Parser = parser;
		}
	}
}