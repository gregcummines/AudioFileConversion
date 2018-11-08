using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Xano.AudioFileConversion
{
	public static class WaveResampler
	{
		public static int GetSampleRate(string filePathInput)
		{
			using (FileStream fs = File.Open(filePathInput, FileMode.Open))
			{
				return ReadStream(fs).sampleRate;
			}
		}

		public static void Resample(string filePathInput, Stream outputStream, int resampleSize)
		{
			if (!File.Exists(filePathInput))
				throw new ArgumentException("Input file does not exist.");

			using (var reader = new WaveFileReader(filePathInput))
			{
				var outFormat = new WaveFormat(resampleSize, reader.WaveFormat.Channels);
				using (var resampler = new MediaFoundationResampler(reader, outFormat))
				{
					// resampler.ResamplerQuality = 60;
					//WaveFileWriter.CreateWaveFile(filePathOutput, resampler);
					WaveFileWriter.WriteWavFileToStream(outputStream, resampler);
                }
			}
		}

		private static (string[] phrasedata, double[] simulatedWaveData) ProcessRawVoiceData(double[] rawData)
		{
			var lastWord = (int) Math.Floor((double)rawData.Length / 4);
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

			var eightBitwordHex = eightBitWord.Select(x => x.ToString("X4")).ToArray();
			//File.WriteAllLines(@"C:\Users\uzzamana\Documents\MATLAB\sample_out\voice_ind_csharp.de", eightBitwordHex.ToArray());

			var decodedSignal = EncodeDecodeHalperFunctions.Decode(encodedSignal);
			var temp = TypeConverter.ConvertDoubleToHex(decodedSignal, 0, 16);
			decodedSignal = TypeConverter.ConvertHexToDouble(temp, 15, 16)/*.Select(x => (int)x).ToArray()*/;
			// We probably shouldn't be doing this, we are upsampling again but aid will still play use 10khz sampling rate. 
			var resampledDecodedSignal = Enumerable.Repeat(0.0, decodedSignal.Length * 2).ToArray();

			for (int i = 0; i < decodedSignal.Length; i++)
			{
				resampledDecodedSignal[i * 2] = decodedSignal[i];
			}

			// Low pass IIR filter as implemented in firmware
			var b_vector = new double[] { 0.0940, 0.3759, 0.5639, 0.3759, 0.0940 };
			var a_vector = new double[] { 1.0000, 0.0000, 0.4860, 0.0000, 0.0177 };
			//b_vector = Hex2Real.Number_Hex2Real(Real2Hex.Number_Real2Hex(b_vector, 23, 24), 23, 24);
			//a_vector = Hex2Real.Number_Hex2Real(Real2Hex.Number_Real2Hex(a_vector, 23, 24), 23, 24);

			resampledDecodedSignal = Filter(b_vector, a_vector, resampledDecodedSignal);
			resampledDecodedSignal = resampledDecodedSignal.Select(x => x * 2).ToArray();

			// Test with Matlab script results.
			// File.WriteAllLines(@"C:\Users\uzzamana\Documents\MATLAB\sample_out\voice_ind_csharp.txt", resampledDecodedSignal.Select(x => x.ToString()));


			return (eightBitwordHex, resampledDecodedSignal);
		}

		public static (int sampleRate, byte[] waveFileHeader, double[] rawWaveData) ReadStream(Stream fs)
		{
			double[] waveFileData = null;
			var waveFileHeader = new List<byte>();
			BinaryReader reader = new BinaryReader(fs);
			
			// chunk 0
			int chunkID = reader.ReadInt32();
			int fileSize = reader.ReadInt32();
			int riffType = reader.ReadInt32();

			waveFileHeader.AddRange(BitConverter.GetBytes(chunkID));
			waveFileHeader.AddRange(BitConverter.GetBytes(fileSize));
			waveFileHeader.AddRange(BitConverter.GetBytes(riffType));

			// chunk 1
			int fmtID = reader.ReadInt32();
			int fmtSize = reader.ReadInt32(); // bytes for this chunk
			short fmtCode = reader.ReadInt16();
			short channels = reader.ReadInt16();
			int sampleRate = reader.ReadInt32();
			int byteRate = reader.ReadInt32();
			short fmtBlockAlign = reader.ReadInt16();
			short bitDepth = reader.ReadInt16();

			waveFileHeader.AddRange(BitConverter.GetBytes(fmtID));
			waveFileHeader.AddRange(BitConverter.GetBytes(fmtSize));
			waveFileHeader.AddRange(BitConverter.GetBytes(fmtCode));
			waveFileHeader.AddRange(BitConverter.GetBytes(channels));
			waveFileHeader.AddRange(BitConverter.GetBytes(sampleRate));
			waveFileHeader.AddRange(BitConverter.GetBytes(byteRate));
			waveFileHeader.AddRange(BitConverter.GetBytes(fmtBlockAlign));
			waveFileHeader.AddRange(BitConverter.GetBytes(bitDepth));

			var res = BitConverter.GetBytes(bitDepth);
			var res2 = Enumerable.Range(0, res.Length / sizeof(Int16))
				.Select(offset => BitConverter.ToInt16(res, offset * sizeof(Int16)))
				.ToArray();

			if (fmtSize == 18)
			{
				// Read any extra values
				short fmtExtraSize = reader.ReadInt16();
				reader.ReadBytes(fmtExtraSize);
			}

			// chunk 2
			int dataID = reader.ReadInt32();
			int bytes = reader.ReadInt32();
			waveFileHeader.AddRange(BitConverter.GetBytes(dataID));
			waveFileHeader.AddRange(BitConverter.GetBytes(bytes));

			// DATA!
			byte[] byteArray = reader.ReadBytes(bytes);

			int bytesForSamp = bitDepth / 8;
			int samps = bytes / bytesForSamp;
			
			switch (bitDepth)
			{
				case 64:
					waveFileData = new double[samps];
					Buffer.BlockCopy(byteArray, 0, waveFileData, 0, bytes);
					waveFileData = Array.ConvertAll(waveFileData, e => e);
					break;
				case 32:
					waveFileData = new double[samps];
					Buffer.BlockCopy(byteArray, 0, waveFileData, 0, bytes);
					break;
				case 16:
					Int16[] asInt16 = new Int16[samps];
					Buffer.BlockCopy(byteArray, 0, asInt16, 0, bytes);
					waveFileData = Array.ConvertAll(asInt16, e => e / (double)Int16.MaxValue);
					break;
				default:
					waveFileData = new double[0];
					break;
			}

			return (sampleRate, waveFileHeader.ToArray(), waveFileData);
		}

        
        private static double[] Filter(double[] b, double[] a, double[] x)
		{
			// This is painfully slow, find/come up with something better.

			double[] result = new double[x.Length];
			result[0] = b[0] * x[0];
			for (int i = 1; i < x.Length; i++)
			{
				result[i] = 0.0;
				int j = 0;
				if ((i < b.Length) && (j < x.Length))
				{
					result[i] += (b[i] * x[j]);
				}
				while (++j <= i)
				{
					int k = i - j;
					if ((k < b.Length) && (j < x.Length))
					{
						result[i] += b[k] * x[j];
					}
					if ((k < x.Length) && (j < a.Length))
					{
						result[i] -= a[j] * result[k];
					}
				}
			}
			return result;
		}
	}
}
