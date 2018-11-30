using System;

namespace Atlas.Core.Utilites
{
	public static class Conversion
	{
		public static double ToRadians(double degrees)
		{
			return Clamp.Radians(degrees * (Math.PI / 180));
		}

		public static double ToDegrees(double radians)
		{
			return Clamp.Degrees(radians * (180 / Math.PI));
		}

		public static double ToDecimal(double number, double min, double max)
		{
			return (number - min) / (max - min);
		}

		public static double FromDecimal(double number, double min, double max)
		{
			return number * (max - min) + min;
		}

		public static double ToPercent(double number, double min, double max)
		{
			return ToDecimal(number, min, max) * 100;
		}

		public static double FromPercent(double number, double min, double max)
		{
			return FromDecimal(number / 100, min, max);
		}

		public static double Ratio(double number, double min1, double max1, double min2, double max2)
		{
			return FromDecimal(ToDecimal(number, min1, max1), min2, max2);
		}
	}
}
