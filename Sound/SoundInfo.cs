using NAudio.Wave;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace ElevatorMusic.Playback
{
    public static class SoundInfo
    {

        /// <summary>
        /// Gets the length of a wav file in seconds.
        /// </summary>
        /// <param name="fileName">The name of the wav file.</param>
        /// <returns>Length in seconds.</returns>
        public static int GetWavLength(string fileName)
        {
            return (int) (0.5 + GetWavLengthMs(fileName) / 1000.0);
        }

        /// <summary>
        /// Gets the length of a wav file in milliseconds.
        /// </summary>
        /// <param name="fileName">The name of the wav file.</param>
        /// <returns>Length in milliseconds.</returns>
        public static long GetWavLengthMs(string fileName)
        {
            return 1000 * GetFileSizeInBits(fileName) / GetWavBitrate(fileName);
        }

        private static long GetFileSizeInBits(string fileName)
        {
            return new FileInfo(fileName).Length * 8;
        }

        /// <summary>
        /// Gets the wav file's bitrate (bits/second).
        /// </summary>
        /// <param name="fileName">The name of the wav file.</param>
        /// <returns>The bitrate (bits/second).</returns>
        public static int GetWavBitrate(string fileName)
        {
            using (var reader = new WaveFileReader(fileName))
            {
                return reader.WaveFormat.AverageBytesPerSecond * 8;
            }
        }

    }
}
