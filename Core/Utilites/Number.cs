using System;
using System.Collections.Generic;

namespace Atlas.Core.Utilites
{
	public static class Number
	{
		public static bool IsEven(double number)
		{
			return (int)number % 2 == 0;
		}

		public static bool IsOdd(double number)
		{
			return !IsEven(number);
		}

		public static bool IsPositive(double number)
		{
			return number >= 0;
		}

		public static bool IsNegative(double number)
		{
			return !IsPositive(number);
		}

		public static bool IsInteger(double number)
		{
			return (int)number == number;
		}

		public static bool Equals(double number1, double number2, double difference = 0)
		{
			return Math.Abs(number1 - number2) <= difference;
		}

		public static bool IsPrime(int integer)
		{
			if(integer == 1)
				return false;
			for(int i = integer / 2; i > 1; --i)
			{
				if(integer % i == 0)
					return false;
			}
			return true;
		}

		public static bool IsInside(double number, double min, double max)
		{
			return (number >= min && number <= max);
		}

		public static bool IsOutside(double number, double min, double max)
		{
			return !IsInside(number, min, max);
		}

		public static bool IsPower(int number, int power)
		{
			if(number == 1)
				return true;
			if(power == 0)
			{
				if(number == 0)
					return true;
				return false;
			}
			var powered = power;
			while(Math.Abs(powered) < Math.Abs(number))
				powered *= power;
			return powered == number;
		}

		public static bool IsMultiple(double number, double multiple)
		{
			return (number % multiple) == 0;
		}

		public static double ClosestMultiple(double number, double multiple)
		{
			return number * Math.Round(multiple / number);
		}

		public static double LowestMultiple(double number, double multiple)
		{
			return number * Math.Floor(multiple / number);
		}

		public static double HighestMultiple(double number, double multiple)
		{
			return number * Math.Ceiling(multiple / number);
		}

		public static int Factorial(int integer)
		{
			return integer < 2 ? 1 : integer * Factorial(integer - 1);
		}

		public static double ClosestNumber(bool asIndex, double number, params double[] numbers)
		{
			int index = numbers.Length - 1;
			double closest = numbers[index];

			for(int i = index - 1; i >= 0; --i)
			{
				double next = numbers[i];
				if(Math.Abs(number - next) < Math.Abs(number - closest))
				{
					index = i;
					closest = next;
				}
			}

			return asIndex ? index : closest;
		}

		public static double Minimum(params double[] numbers)
		{
			double minimum = numbers[0];
			foreach(var number in numbers)
			{
				if(number < minimum)
					minimum = number;
			}
			return minimum;
		}

		public static double Maximum(params double[] numbers)
		{
			double maximum = numbers[0];
			foreach(var number in numbers)
			{
				if(number > maximum)
					maximum = number;
			}
			return maximum;
		}

		public static double Sum(params double[] numbers)
		{
			double sum = 0;
			foreach(var number in numbers)
				sum += number;
			return sum;
		}

		public static double Mean(params double[] numbers)
		{
			return Sum(numbers) / numbers.Length;
		}

		public static double Median(params double[] numbers)
		{
			numbers = (double[])numbers.Clone();
			Array.Sort(numbers);
			var length = numbers.Length;

			if(IsOdd(length))
			{
				return numbers[(length - 1) / 2];
			}
			else
			{
				return (numbers[length / 2] + numbers[(length / 2) - 1]) / 2;
			}
		}

		public static double[] Mode(params double[] numbers)
		{
			var counts = new Dictionary<double, int>();
			int mode = 0;
			double[] modes = new double[1];

			foreach(var number in numbers)
			{
				if(counts.ContainsKey(number))
				{
					counts[number] = 0;
				}
				else
				{
					counts[number] += 1;
				}
				if(counts[number] > mode)
				{
					mode = counts[number];
				}
			}

			foreach(var number in counts.Keys)
			{
				if(counts[number] == mode)
				{
					modes[modes.Length] = number;
				}
			}

			return modes;
		}

		public static double Range(params double[] numbers)
		{
			return Maximum(numbers) - Minimum(numbers);
		}

		/// <summary>
		/// Greatest common divisor.
		/// </summary>
		/// <param name="integer1"></param>
		/// <param name="integer2"></param>
		/// <returns></returns>
		public static int GCD(int integer1, int integer2)
		{
			return integer2 == 0 ? integer1 : GCD(integer2, integer1 % integer2);
		}

		/// <summary>
		/// Least common multiple.
		/// </summary>
		/// <param name="integer1"></param>
		/// <param name="integer2"></param>
		/// <returns></returns>
		public static int LCM(int integer1, int integer2)
		{
			return integer1 * integer2 / GCD(integer1, integer2);
		}
	}
}