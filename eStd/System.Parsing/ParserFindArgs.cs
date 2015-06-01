using System;
using System.Parsing;

namespace System.Parsing
{
	public class ParserFindArgs : ParserChain
	{
		public string ParserId { get; set; }

		public ParserFindArgs(string parserId)
		{
			this.ParserId = parserId;
		}
	}
}