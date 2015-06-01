using System;
using System.Parsing;

namespace System.Parsing.Parsers
{
	public class SymbolTerminal : CharTerminal
	{
		protected SymbolTerminal(SymbolTerminal other, ParserCloneArgs args)
			: base(other, args)
		{
		}

		public SymbolTerminal()
		{
		}

		protected override bool Test(char ch)
		{
			return Char.IsSymbol(ch);
		}

		protected override string CharName
		{
			get { return "Symbol"; }
		}
		
		public override Parser Clone(ParserCloneArgs args)
		{
			return new SymbolTerminal(this, args);
		}
	}
}