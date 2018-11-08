using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xano.AudioFileConversion
{
	public static class TypeConverter
	{
		public static double[] ConvertHexToDouble(string[] input, int FractionLength, int WordLength = 24)
		{
			if (FractionLength >= WordLength)
			{
				throw new Exception("fractional bits should be less than word length");
			}

			var convertedData = new List<double>(input.Length);
			foreach (var hexString in input)
			{
				var val = int.Parse(hexString, System.Globalization.NumberStyles.HexNumber);
				convertedData.Add(val * Math.Pow(2, -FractionLength));
			}

			return convertedData.ToArray();
		}

		public static string[] ConvertDoubleToHex(double[] input, int FractionLength = 15, int WordLength = 24)
		{
			if (FractionLength >= WordLength)
			{
				throw new Exception("fractional bits should be less than word length");
			}

			var convertedData = new List<string>(input.Length);
			foreach (var item in input)
			{
				var val = ((int)Math.Round(item * Math.Pow(2, FractionLength))).ToString("X4");
				convertedData.Add(val);
			}
			return convertedData.ToArray();
		}		
	}
}
