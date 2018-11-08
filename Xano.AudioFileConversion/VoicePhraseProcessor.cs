using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xano.AudioFileConversion
{
    public static class VoicePhraseProcessor
    {
        public static void ProcessWaveFile(string inputFilePath, string outputFolder, int sampleRate)
        {
            var waveFileData = new double[0];
            var waveFileHeader = new byte[0];

            var fileNameNoExt = Path.GetFileNameWithoutExtension(inputFilePath);
            var newFilePath = Path.Combine(outputFolder, fileNameNoExt + "_simluated.wav");
            var edfFilePath = Path.Combine(outputFolder, fileNameNoExt + ".edf");
            using (var outputStream = new MemoryStream())
            {
                WaveResampler.Resample(inputFilePath, outputStream, sampleRate);
                outputStream.Position = 0;
                (sampleRate, waveFileHeader, waveFileData) = WaveResampler.ReadStream(outputStream);
                
                var (firmwarePhraseData, simulatedData) = ProcessRawVoiceData(waveFileData);

                File.WriteAllText(edfFilePath, string.Join("", firmwarePhraseData));

                WaveFormat waveFormat = new WaveFormat(20000, 1);
                using (WaveFileWriter writer = new WaveFileWriter(newFilePath, waveFormat))
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
