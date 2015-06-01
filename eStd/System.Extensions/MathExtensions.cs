using System.Collections.Generic;

namespace System.Extensions
{
    public static class MathExtensions
    {

        public static int Fibonacci(this int n)
        {
            int a = 0;
            int b = 1;
            // In N steps compute Fibonacci sequence iteratively.
            for (int i = 0; i < n; i++)
            {
                int temp = a;
                a = b;
                b = temp + b;
            }
            return a;
        }

        public static int[] Fibonaccis(this int n)
        {
            var r = new List<int>();

            for (int i = 1; i < n+1; i++)
            {
                r.Add(Fibonacci(i));
            }

            return r.ToArray();
        }

    }
}
