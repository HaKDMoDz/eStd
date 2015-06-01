using System;
using System.Parsing;

namespace System.Parsing.Parsers
{
	public class LetterTerminal : CharTerminal
	{
		protected LetterTerminal(LetterTerminal other, ParserCloneArgs args)
			: base(other, args)
		{
		}

		public LetterTerminal()
		{
		}

		protected override bool Test(char ch)
		{
			return Char.IsLetter(ch);
		}

		protected override string CharName
		{
			get { return "Letter"; }
		}
		
		public override Parser Clone(ParserCloneArgs args)
		{
			return new LetterTerminal(this, args);
		}
	}
}