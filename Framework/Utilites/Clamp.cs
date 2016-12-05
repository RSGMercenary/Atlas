using System;

namespace Atlas.Framework.Utilites
{
	class Clamp
	{
		/// <summary>
		/// Clamps radians to be between +/-3.14.
		/// </summary>
		/// <param name="radians"></param>
		/// <returns></returns>
		public static double Radians(double radians)
		{
			while(radians > Math.PI)
				radians -= Math.PI * 2;
			while(radians < -Math.PI)
				radians += Math.PI * 2;
			return radians;
		}

		/// <summary>
		/// Clamps degrees to be between +/-180.
		/// </summary>
		/// <param name="degrees"></param>
		/// <returns></returns>
		public static double Degrees(double degrees)
		{
			while(degrees > 180)
				degrees -= 360;
			while(degrees < -180)
				degrees += 360;
			return degrees;
		}

		/// <summary>
		/// Clamps the number inclusively inside min and max.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public static double Inside(double number, double min, double max)
		{
			if(number < min)
				return min;
			if(number > max)
				return max;
			return number;
		}

		/// <summary>
		/// Clamps the number inclusively outside min and max.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public static double Outside(double number, double min, double max)
		{
			if(number <= min)
				return number;
			if(number >= max)
				return number;
			return Math.Abs(number - min) < Math.Abs(number - max) ? min : max;
		}
	}
}
