using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace day12_3BodyProblem
{
    // https://stackoverflow.com/a/13731775
    public class MathUtils
    {
        /// <summary>
        /// Calculates the least common multiple of 2+ numbers.
        /// </summary>
        /// <remarks>
        /// Uses recursion based on lcm(a,b,c) = lcm(a,lcm(b,c)).
        /// Ported from http://stackoverflow.com/a/2641293/420175.
        /// </remarks>
        public static Int64 LCM(IList<Int64> numbers)
        {
            if (numbers.Count < 2)
                throw new ArgumentException("you must pass two or more numbers");
            return LCM(numbers, 0);
        }

        public static Int64 LCM(params Int64[] numbers)
        {
            return LCM((IList<Int64>)numbers);
        }

        private static Int64 LCM(IList<Int64> numbers, int i)
        {
            // Recursively iterate through pairs of arguments
            // i.e. lcm(args[0], lcm(args[1], lcm(args[2], args[3])))

            if (i + 2 == numbers.Count)
            {
                return LCM(numbers[i], numbers[i + 1]);
            }
            else
            {
                return LCM(numbers[i], LCM(numbers, i + 1));
            }
        }

        public static Int64 LCM(Int64 a, Int64 b)
        {
            return (a * b / GCD(a, b));
        }

        /// <summary>
        /// Finds the greatest common denominator for 2 numbers.
        /// </summary>
        /// <remarks>
        /// Also from http://stackoverflow.com/a/2641293/420175.
        /// </remarks>
        public static Int64 GCD(Int64 a, Int64 b)
        {
            // Euclidean algorithm
            Int64 t;
            while (b != 0)
            {
                t = b;
                b = a % b;
                a = t;
            }
            return a;
        }
    }
}
