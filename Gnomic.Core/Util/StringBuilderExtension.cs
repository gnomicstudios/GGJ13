using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gnomic.Util
{
    /// <summary>
    /// Adds append functions for writing decimal and integer numbers to
    /// StringBuilder. This is useful as string.Format generates garbage.
    /// </summary>
    public static class StringBuilderExtension
    {
        static char[] numberBuffer = new char[16];

        /// <summary>
        /// Append a floating point number with the given number of decimal places
        /// </summary>
        public static void AppendNumber(this StringBuilder builder, float number, int dp)
        {
            int intPart = (int)number;
            int floatPart = (int)((number - (float)intPart) * Math.Pow(10, dp));
            AppendNumber(builder, intPart);
            builder.Append('.');
            AppendNumber(builder, floatPart, dp);
        }

        /// <summary>
        /// Append an integer
        /// </summary>
        public static void AppendNumber(this StringBuilder builder, int number)
        {
            AppendNumber(builder, number, 0);
        }

        /// <summary>
        /// Appends an integer with the minimum number of digits (padded with zeroes)
        /// </summary>
        public static void AppendNumber(this StringBuilder builder, int number, int minDigits)
        {
            if (number < 0)
            {
                builder.Append('-');
                number = -number;
            }
            int index = 0;
            do
            {
                int digit = number % 10;
                numberBuffer[index] = (char)('0' + digit);
                number /= 10;
                ++index;
            } while (number > 0 || index < minDigits);

            for (--index; index >= 0; --index)
            {
                builder.Append(numberBuffer[index]);
            }
        } 
    }
}
