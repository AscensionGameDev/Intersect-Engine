using System;

using Intersect.Client.Framework.Audio;
using Intersect.Client.Interface.Game.Chat;
using Intersect.Client.Localization;
using Intersect.Client.Utilities;
using Intersect.Logging;

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

using NVorbis;

namespace Intersect.Client.MonoGame.Audio
{

    public class MonoMusicSource : GameAudioSource
    {

        private readonly string mPath;

        private VorbisReader mReader;

        private DynamicSoundEffectInstance mInstance;

        public MonoMusicSource(string path)
        {
            mPath = path;
        }

        public override GameAudioInstance CreateInstance()
        {
            return new MonoMusicInstance(this);
        }

        public DynamicSoundEffectInstance LoadSong()
        {
            if (!string.IsNullOrWhiteSpace(mPath))
            {
                try
                {
                    if (mReader == null)
                    {
                        mReader = new VorbisReader(mPath);
                    }

                    if (mInstance != null)
                    {
                        mInstance.Dispose();
                        mInstance = null;
                    }

                    mInstance = new DynamicSoundEffectInstance(mReader.SampleRate, mReader.Channels == 1 ? AudioChannels.Mono : AudioChannels.Stereo);
                    mInstance.BufferNeeded += BufferNeeded;


                    return mInstance;
                }
                catch (Exception exception)
                {
                    Log.Error($"Error loading '{mPath}'.", exception);
                    ChatboxMsg.AddMessage(
                        new ChatboxMsg(
                            Strings.Errors.LoadFile.ToString(Strings.Words.lcase_sound), new Color(0xBF, 0x0, 0x0)
                        )
                    );
                }
            }

            return null;
        }

        public void Close()
        {
            if (mReader != null)
            {
                mReader.Dispose();
                mReader = null;
            }
        }

        private void BufferNeeded(object sender, EventArgs args)
            => FillBuffers();

        private void FillBuffers(int buffers = 3, int samples = 44100)
        {
            float[] sampleBuffer = null;

            while (mInstance.PendingBufferCount < buffers && mReader != null)
            {
                if (sampleBuffer == null)
                    sampleBuffer = new float[samples];

                var read = mReader.ReadSamples(sampleBuffer, 0, sampleBuffer.Length);
                if (read == 0)
                {
                    mReader.DecodedPosition = 0;
                    continue;
                }

                var dataBuffer = new byte[read << 1];
                for (var sampleIndex = 0; sampleIndex < read; ++sampleIndex)
                {
                    var sample = (short)MathHelper.Clamp(sampleBuffer[sampleIndex] * 32767f, short.MinValue, short.MaxValue);
                    var sampleData = BitConverter.GetBytes(sample);
                    for (var sampleByteIndex = 0; sampleByteIndex < sampleData.Length; ++sampleByteIndex)
                        dataBuffer[(sampleIndex << 1) + sampleByteIndex] = sampleData[sampleByteIndex];
                }

                mInstance.SubmitBuffer(dataBuffer, 0, read << 1);
            }
        }

    }

}
