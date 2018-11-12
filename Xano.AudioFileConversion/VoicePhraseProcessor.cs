using NAudio.Wave;
using NWaves.Filters.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Xano.AudioFileConversion
{
	public static class VoicePhraseProcessor
	{
		public static void ProcessWaveFile(string inputFilePath, string outputFolder, int sampleRate)
		{
			var waveFileData = new double[0];
			var waveFileHeader = new byte[0];

			var fileNameNoExt = Path.GetFileNameWithoutExtension(inputFilePath);

			var resampledFilePath = Path.Combine(outputFolder, fileNameNoExt + "_resampled.wav");
			var simulatedFilePath = Path.Combine(outputFolder, fileNameNoExt + "_simluated.wav");
			var edfFilePath = Path.Combine(outputFolder, fileNameNoExt + ".edf");
			using (var outputStream = new MemoryStream())
			{
				WaveResampler.Resample(inputFilePath, outputStream, sampleRate);

				var fileStream = File.Create(resampledFilePath);
				outputStream.Seek(0, SeekOrigin.Begin);
				outputStream.CopyTo(fileStream);
				fileStream.Close();

				outputStream.Position = 0;
				(sampleRate, waveFileHeader, waveFileData) = WaveResampler.ReadStream(outputStream);
				
				var (firmwarePhraseData, simulatedData) = ProcessRawVoiceData(waveFileData);

				File.WriteAllText(edfFilePath, string.Join("", firmwarePhraseData));
				
				WaveFormat waveFormat = new WaveFormat(10000, 1);
				using (WaveFileWriter writer = new WaveFileWriter(simulatedFilePath, waveFormat))
				{
					float[] floatArray = Array.ConvertAll(simulatedData, x => (float)x);
					writer.WriteSamples(floatArray, 0, floatArray.Length);
				}
			}
		}

		private static (string[] phrasedata, double[] simulatedWaveData) ProcessRawVoiceData(double[] rawData)
		{
			var lastWord = (int)Math.Floor((double)rawData.Length / 4);
			var clippedRawData = new double[lastWord * 4];

			var maxValue = 0.0;
			for (int i = 0; i < clippedRawData.Length; i++)
			{
				clippedRawData[i] = rawData[i];

				if (Math.Abs(clippedRawData[i]) > maxValue)
				{
					maxValue = Math.Abs(clippedRawData[i]);
				}
			}

			var multiplier = TypeConverter.ConvertHexToDouble(new string[] { "7FF0" }, 15, 16).First();
			clippedRawData = clippedRawData.Select(x => (x / maxValue) * multiplier).ToArray();

			var encodedSignal = EncodeDecodeHalperFunctions.Encode(clippedRawData);
			var signalLength = encodedSignal.Length / 2;
			var eightBitWord = new List<int>(signalLength);
			for (int i = 0; i < signalLength; i++)
			{
				var v1 = ((int)encodedSignal[2 * i]) << 4;
				var v2 = v1 | (int)encodedSignal[2 * i + 1];
				eightBitWord.Add(v2);
			}

			var eightBitwordHex = eightBitWord.Select(x => x.ToString("X2")).ToArray();
			//File.WriteAllLines(@"C:\Users\uzzamana\Documents\MATLAB\sample_out\voice_ind_csharp.de", eightBitwordHex.ToArray());

			var decodedSignal = EncodeDecodeHalperFunctions.Decode(encodedSignal);
			var temp = TypeConverter.ConvertDoubleToHex(decodedSignal, 0, 16);
			decodedSignal = TypeConverter.ConvertHexToDouble(temp, 15, 16);
			
			// Low pass IIR filter as implemented in firmware
			var b_vector = new double[] { 0.0940, 0.3759, 0.5639, 0.3759, 0.0940 };
			var a_vector = new double[] { 1.0000, 0.0000, 0.4860, 0.0000, 0.0177 };
			//b_vector = TypeConverter.ConvertHexToDouble(TypeConverter.ConvertDoubleToHex(b_vector, 23, 24), 23, 24);
			//a_vector = TypeConverter.ConvertHexToDouble(TypeConverter.ConvertDoubleToHex(a_vector, 23, 24), 23, 24);

			decodedSignal = Filter(b_vector, a_vector, decodedSignal);
			
			for (int i = 0; i < decodedSignal.Length; i++)
			{
				if (decodedSignal[i] >= 1)
				{
					decodedSignal[i] = 0.99;
				}
			}

			// Test with Matlab script results.
			// File.WriteAllLines(@"C:\Users\uzzamana\Documents\MATLAB\sample_out\voice_ind_csharp.txt", decodedSignal.Select(x => x.ToString()));

			return (eightBitwordHex, decodedSignal);
		}

		private static double[] Filter(double[] b, double[] a, double[] x)
		{
			var filter = new IirFilter(b, a);
			var input = Array.ConvertAll(x, r => (float)r);
			var output = filter.Process(input, FilteringOptions.Auto);
			var convertedOutput = Array.ConvertAll(output, r => (double)r);
			return convertedOutput;

			// This is painfully slow, find/come up with something better.

			//double[] result = new double[x.Length];
			//result[0] = b[0] * x[0];
			//for (int i = 1; i < x.Length; i++)
			//{
			//    result[i] = 0.0;
			//    int j = 0;
			//    if ((i < b.Length) && (j < x.Length))
			//    {
			//        result[i] += (b[i] * x[j]);
			//    }
			//    while (++j <= i)
			//    {
			//        int k = i - j;
			//        if ((k < b.Length) && (j < x.Length))
			//        {
			//            result[i] += b[k] * x[j];
			//        }
			//        if ((k < x.Length) && (j < a.Length))
			//        {
			//            result[i] -= a[j] * result[k];
			//        }
			//    }
			//}
			//return result;
		}
	}
}
