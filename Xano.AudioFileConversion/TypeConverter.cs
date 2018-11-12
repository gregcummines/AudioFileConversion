using System;
using System.Collections.Generic;

namespace Xano.AudioFileConversion
{
	public static class TypeConverter
	{
		public static double[] ConvertHexToDouble(string[] input, int fractionLength, int wordLength = 24)
		{
			if (fractionLength >= wordLength)
			{
				throw new Exception("fractional bits should be less than word length");
			}

			var convertedData = new List<double>(input.Length);
			foreach (var hexString in input)
			{
				var val = int.Parse(hexString, System.Globalization.NumberStyles.HexNumber);
				convertedData.Add(val * Math.Pow(2, -fractionLength));
			}

			return convertedData.ToArray();
		}

		public static string[] ConvertDoubleToHex(double[] input, int fractionLength = 15, int wordLength = 24)
		{
			if (fractionLength >= wordLength)
			{
				throw new Exception("fractional bits should be less than word length");
			}

			var convertedData = new List<string>(input.Length);
			foreach (var item in input)
			{
				var y = (int) (item * Math.Pow(2, fractionLength));
				var upperLimit = (int)  Math.Pow(2, wordLength - 1) - 1;
				var lowerLimit = (int) Math.Pow(-2, wordLength - 1) - 1;

				if (y > upperLimit)
				{
					y = upperLimit;
				}
				else if (y < lowerLimit)
				{
					y = lowerLimit;
				}

				var val = y.ToString("X4");
				convertedData.Add(val);
			}
			return convertedData.ToArray();
		}		
	}
}
