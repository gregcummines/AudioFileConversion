using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xano.AudioFileConversion
{
    public class WaveResampler
    {
        private static int GetSampleRate(string filePathInput)
        {
            var sampleRate = 0;
            
            using (FileStream fs = File.Open(filePathInput, FileMode.Open))
            {
                BinaryReader reader = new BinaryReader(fs);

                // chunk 0
                int chunkID = reader.ReadInt32();
                int fileSize = reader.ReadInt32();
                int riffType = reader.ReadInt32();


                // chunk 1
                int fmtID = reader.ReadInt32();
                int fmtSize = reader.ReadInt32(); // bytes for this chunk
                int fmtCode = reader.ReadInt16();
                int channels = reader.ReadInt16();
                sampleRate = reader.ReadInt32();
                int byteRate = reader.ReadInt32();
                int fmtBlockAlign = reader.ReadInt16();
                int bitDepth = reader.ReadInt16();
            }
            return sampleRate;
        }

        private static void Resample(string filePathInput, string filePathOutput, int resampleSize)
        {
            if (filePathInput == filePathOutput)
                throw new ArgumentException("Input and Output file paths cannot be the same.");

            if (!File.Exists(filePathInput))
                throw new ArgumentException("Input file does not exist.");

            using (var reader = new WaveFileReader(filePathInput))
            {
                var outFormat = new WaveFormat(resampleSize, reader.WaveFormat.Channels);
                using (var resampler = new MediaFoundationResampler(reader, outFormat))
                {
                    // resampler.ResamplerQuality = 60;
                    WaveFileWriter.CreateWaveFile(filePathOutput, resampler);
                }
            }
        }
    }
}
