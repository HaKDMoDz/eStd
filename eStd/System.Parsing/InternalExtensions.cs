using System;

namespace System.Parsing
{
	static class InternalExtensions
	{
		internal static void ThrowIfNull<T>(this T o, string paramName, string message = null) where T : class
		{
			if (o == null)
				throw new ArgumentNullException(paramName, message);
		}
	}
}