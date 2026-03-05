using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;

namespace Client.MirSounds.Libraries
{
    class CachedSound
    {
        public int Index { get; private set; }
        public long ExpireTime { get; set; }
        public float[] AudioData { get; private set; }
        public WaveFormat WaveFormat { get; private set; }

        public CachedSound(int index, string fileName)
        {
            Index = index;

            fileName = Path.Combine(Settings.SoundPath, fileName);
            string fileType = Path.GetExtension(fileName);

            // attempt to find file
            if (String.IsNullOrEmpty(fileType))
            {
                foreach (String ext in SoundManager.SupportedFileTypes)
                {
                    if (File.Exists($"{fileName}{ext}"))
                    {
                        fileName = $"{fileName}{ext}";
                        fileType = ext;

                        break;
                    }
                }
            }

            if (SoundManager.SupportedFileTypes.Contains(fileType) &&
                File.Exists(fileName))
            {
                using (var audioFileReader = new AudioFileReader(fileName))
                {
                    // 确保所有缓存的音效与全局混音器使用相同的采样率
                    ISampleProvider source = audioFileReader;
                    if (audioFileReader.WaveFormat.SampleRate != SoundManager.SampleRate)
                    {
                        source = new WdlResamplingSampleProvider(source, SoundManager.SampleRate);
                    }

                    WaveFormat = source.WaveFormat;

                    var wholeFile = new List<float>();
                    var readBuffer = new float[WaveFormat.SampleRate * WaveFormat.Channels];
                    int samplesRead;
                    while ((samplesRead = source.Read(readBuffer, 0, readBuffer.Length)) > 0)
                    {
                        wholeFile.AddRange(readBuffer.Take(samplesRead));
                    }

                    AudioData = wholeFile.ToArray();
                }
            }
        }
    }
}
