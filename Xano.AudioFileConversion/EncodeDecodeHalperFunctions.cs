using System;
using System.Collections.Generic;

namespace Xano.AudioFileConversion
{
	public static class EncodeDecodeHalperFunctions
	{
		private static readonly int[] indexTable = new int[] { -1, -1, -1, -1, 2, 4, 6, 8, -1, -1, -1, -1, 2, 4, 6, 8 };
		private static readonly int[] stepSizeTable = new int[] {
		00007, 00008, 00009, 00010, 00011, 00012, 00013, 00014,

		00016, 00017, 00019, 00021, 00023, 00025, 00028, 00031,

		00034, 00037, 00041, 00045, 00050, 00055, 00060, 00066, 

		00073, 00080, 00088, 00097, 00107, 00118, 00130, 00143, 

		00157, 00173, 00190, 00209, 00230, 00253, 00279, 00307, 

		00337, 00371, 00408, 00449, 00494, 00544, 00598, 00658, 

		00724, 00796, 00876, 00963, 01060, 01166, 01282, 01411, 

		01552, 01707, 01878, 02066, 02272, 02499, 02749, 03024, 

		03327, 03660, 04026, 04428, 04871, 05358, 05894, 06484, 

		07132, 07845, 08630, 09493, 10442, 11487, 12635, 13899, 

		15289, 16818, 18500, 20350, 22385, 24623, 27086, 29794, 

		32767 };

		public static double[] Encode(double[] speechFile)
		{
			// Init Variables
			var prevsample = 0;
			var previndex = 1;
			var numberOfSamples = speechFile.Length;
			var n = 1;
			var result = new List<double>(speechFile.Length);

			var temp = TypeConverter.ConvertDoubleToHex(speechFile, 15, 16);
			speechFile = TypeConverter.ConvertHexToDouble(temp, 0, 16);

			for (int i = 0; i < numberOfSamples; i++)
			{
				var predsample = prevsample;
				var index = previndex;

				var step = stepSizeTable[index - 1];

				var diff = speechFile[n - 1] - predsample;
				var code = diff >= 0 ? 0 : 8;
				diff = Math.Abs(diff);

				var timeStep = step;
				if (diff >= timeStep)
				{
					code = code | 4;
					diff = diff - timeStep;
				}

				timeStep = timeStep >> 1;
				if (diff >= timeStep)
				{
					code = code | 2;
					diff = diff - timeStep;
				}

				timeStep = timeStep >> 1;
				if (diff >= timeStep)
				{
					code = code | 1;
				}
				
				var diffq = step >> 3;
				if ((code & 4) != 0)
				{
					diffq = diffq + step;
				}

				if ((code & 2) != 0)
				{
					diffq = diffq + (step >> 1);
				}

				if ((code & 1) != 0)
				{
					diffq = diffq + (step >> 2);
				}

				if ((code & 8) != 0)
				{
					predsample = predsample - diffq;
				}
				else
				{
					predsample = predsample + diffq;
				}

				if (predsample > 32767)
				{
					predsample = 32767;
				}
				else if (predsample < -32768)
				{
					predsample = -32768;
				}

				index = index + indexTable[code];

				if (index < 1)
				{
					index = 1;
				}
				else if (index >= stepSizeTable.Length)
				{
					index = stepSizeTable.Length - 1;
				}


				prevsample = predsample;
				previndex = index;

				var val = code & 15;
				result.Add(val);
				n = n + 1;
			}
			return result.ToArray();
		}

		public static double[] Decode(double[] adpcm)
		{
			// Init Variables
			var prevsample = 0;
			var previndex = 1;
			var numberOfSamples = adpcm.Length;
			var n = 1;
			var result = new List<double>(adpcm.Length);

			for (int i = 0; i < numberOfSamples; i++)
			{
				var predsample = prevsample;
				var index = previndex;

				var step = stepSizeTable[index - 1];

				var code = adpcm[n - 1];

				var diffq = step >> 3;
				if (((int)code & 4) != 0)
				{
					diffq = diffq + step;
				}

				if (((int)code & 2) != 0)
				{
					diffq = diffq + (step >> 1);
				}

				if (((int)code & 1) != 0)
				{
					diffq = diffq + (step >> 2);
				}

				if (((int)code & 8) != 0)
				{
					predsample = predsample - diffq;
				}
				else
				{
					predsample = predsample + diffq;
				}

				if (predsample > 32767)
				{
					predsample = 32767;
				}
				else if (predsample < -32768)
				{
					predsample = -32768;
				}

				index = index + indexTable[(int)code];

				if (index < 1)
				{
					index = 1;
				}
				else if (index >= stepSizeTable.Length)
				{
					index = stepSizeTable.Length - 1;
				}
				
				prevsample = predsample;
				previndex = index;

				result.Add(predsample);
				n = n + 1;
			}
			return result.ToArray();
		}
	}
}
