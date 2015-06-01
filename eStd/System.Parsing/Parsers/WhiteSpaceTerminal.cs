using System;
using System.Parsing;

namespace System.Parsing.Parsers
{
	public class WhiteSpaceTerminal : CharTerminal
	{
		protected WhiteSpaceTerminal(WhiteSpaceTerminal other, ParserCloneArgs args)
			: base(other, args)
		{
		}

		public WhiteSpaceTerminal()
		{
		}

		protected override bool Test(char ch)
		{
			return Char.IsWhiteSpace(ch);
		}

		protected override string CharName
		{
			get { return "White Space"; }
		}
		
		public override Parser Clone(ParserCloneArgs args)
		{
			return new WhiteSpaceTerminal(this, args);
		}
	}
}