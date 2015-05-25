using System;

namespace AppPlatform.Core.EnterpriseLibrary.Extensions.DecimalExtensions
{
    public static class DecimalExtensions
    {
        public static decimal RoundOnExactDecimals(this decimal value, int maxDecimals, MidpointRounding mode)
        {
            var fractionPositions = GetExactFractionPosition(value);
            if (fractionPositions <= maxDecimals)
            {
                return Math.Round(value, fractionPositions);
            }
            return Math.Round(value, maxDecimals, mode);
        }

        public static decimal RoundOnExactDecimals(this decimal value)
        {
            var fractionPositions = GetExactFractionPosition(value);

            return Math.Round(value, fractionPositions);
        }

        private static int GetExactFractionPosition(decimal value)
        {
            var decPositions = 0;

            value = value - ((long)value); //Get initial faraction value;

            while (value != 0m)
            {
                decPositions++;

                var value1 = value * 10;
                value = value1 - ((long)value1); //Get new faraction value;
            }

            return decPositions;
        }
    }
}
