﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xano.AudioFileConversion
{
	public sealed class AudioFormat
	{
		int averageBytesPerSecond;
		int bitsPerSample;
		int blockAlign;
		int channelCount;
		int format;
		internal List<byte> nativeWaveFormat;
		int sampleRate;

		/// <summary>
		/// Gets the average bytes processed per second.
		/// </summary>
		/// <value>Average bytes processed per second.</value>
		public int AverageBytesPerSecond { get { return averageBytesPerSecond; } }

		/// <summary>
		/// Gets the bit depth of the audio content.
		/// </summary>
		/// <value>If the audio has not been processed, the source bit depth; otherwise, the bit depth of the new format.</value>
		public int BitsPerSample { get { return bitsPerSample; } }

		/// <summary>
		/// Gets the number of bytes per sample block, taking channels into consideration. For example, for 16-bit stereo audio (PCM format), the size of each sample block is 4 bytes.
		/// </summary>
		/// <value>Number of bytes, per sample block.</value>
		public int BlockAlign { get { return blockAlign; } }

		/// <summary>
		/// Gets the number of channels.
		/// </summary>
		/// <value>If the audio has not been processed, the source channel count; otherwise, the new channel count.</value>
		public int ChannelCount { get { return channelCount; } }

		/// <summary>
		/// Gets the format of the audio content.
		/// </summary>
		/// <value>If the audio has not been processed, the format tag of the source content; otherwise, the new format tag.</value>
		public int Format { get { return format; } }

		/// <summary>
		/// Gets the raw byte buffer for the format. For non-PCM formats, this buffer contains important format-specific information beyond the basic format information exposed in other properties of the AudioFormat type.
		/// </summary>
		/// <value>The raw byte buffer represented in a collection.</value>
		//public ReadOnlyCollection<byte> NativeWaveFormat { get { return nativeWaveFormat.AsReadOnly(); } }

		/// <summary>
		/// Gets the sample rate of the audio content.
		/// </summary>
		/// <value>If the audio has not been processed, the source sample rate; otherwise, the new sample rate.</value>
		public int SampleRate { get { return sampleRate; } }

        /// <summary>
        /// Creates a new instance of the AudioFormat class
        /// </summary>
        /// <param name="waveFormat">The WaveFormat representing the WAV header.</param>
        internal AudioFormat(NAudio.Wave.WaveFormat waveFormat)
        {
            averageBytesPerSecond = waveFormat.AverageBytesPerSecond;
            bitsPerSample = waveFormat.BitsPerSample;
            blockAlign = waveFormat.BlockAlign;
            channelCount = waveFormat.Channels;
            format = (int)waveFormat.Encoding;
            sampleRate = waveFormat.SampleRate;

            var stream = new MemoryStream();
            using (var writer = new BinaryWriter(stream))
            {
                waveFormat.Serialize(writer);
                nativeWaveFormat = new List<byte>(stream.ToArray());
            }
        }

	}
}
