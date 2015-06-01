using System;
using System.Parsing;

namespace System.Parsing.Parsers
{
	public class SingleLineWhiteSpaceTerminal : CharTerminal
	{
		protected SingleLineWhiteSpaceTerminal(SingleLineWhiteSpaceTerminal other, ParserCloneArgs args)
			: base(other, args)
		{
		}

		public SingleLineWhiteSpaceTerminal()
		{
		}

		protected override bool Test(char ch)
		{
			return ch != '\n' && ch != '\r' && Char.IsWhiteSpace(ch);
		}

		protected override string CharName
		{
			get { return "Single Line White Space"; }
		}
		
		public override Parser Clone(ParserCloneArgs args)
		{
			return new SingleLineWhiteSpaceTerminal(this, args);
		}
	}
}