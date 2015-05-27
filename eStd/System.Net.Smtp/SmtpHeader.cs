namespace System.Net.Smtp
{
	public class SmtpHeader
	{
		private readonly String[] _restricted = {
			"to", "from", "reply-to", "subject", "content-type", "mime-version"
																		 };
		public String Name { get; set; }
		public String Value { get; set; }

		public SmtpHeader(String n, String v)
		{
			foreach (String r in _restricted)
			{
				if (r == n.ToLower()) throw new SmtpException(n + ": restricted header");
			}

			Name = n;
			Value = v;
		}
	}
}