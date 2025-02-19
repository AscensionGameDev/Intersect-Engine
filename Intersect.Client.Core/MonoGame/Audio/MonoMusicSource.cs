using System.Runtime.CompilerServices;
using Intersect.Client.Framework.Audio;
using Intersect.Client.General;
using Intersect.Client.Interface.Game.Chat;
using Intersect.Client.Localization;
using Intersect.Client.Utilities;
using Intersect.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework.Audio;
using NVorbis;

namespace Intersect.Client.MonoGame.Audio;


public partial class MonoMusicSource : GameAudioSource
{
    private static Thread mUnderlyingThread;
    private static readonly object mInstanceLock = new();
    private static MonoMusicSource? mActiveSource;

    private readonly string mPath;
    private readonly string mRealPath;
    private readonly Func<Stream> mCreateStream;
    private DynamicSoundEffectInstance? _instance;

    public VorbisReader? Reader { get; set; }

    public DynamicSoundEffectInstance? Instance
    {
        get => _instance;
        set
        {
            if (value == _instance)
            {
                return;
            }

            _instance = value;
            if (value == null)
            {
                EmitUnloaded();
            }
            else
            {
                EmitLoaded();
            }
        }
    }

    public MonoMusicSource(string path, string realPath, string? name = default)
    {
        Name = string.IsNullOrWhiteSpace(name) ? string.Empty : name;
        mPath = path;
        mRealPath = realPath;

        InitializeThread();
    }

    public MonoMusicSource(Func<Stream> createStream, string? name = default)
    {
        Name = string.IsNullOrWhiteSpace(name) ? string.Empty : name;
        mCreateStream = createStream;

        InitializeThread();
    }

    private void InitializeThread()
    {
        //if (mUnderlyingThread == null)
        //{
        //    mUnderlyingThread = new Thread(EnsureBuffersFilled)
        //    {
        //        Priority = ThreadPriority.Lowest,
        //        IsBackground = true
        //    };

        //    mUnderlyingThread.Start();
        //}
    }

    public override bool IsLoaded => Instance != null;

    public override int TypeVolume => Globals.Database.MusicVolume;

    public override GameAudioInstance CreateInstance()
    {
        return new MonoMusicInstance(this);
    }

    public override void ReleaseInstance(GameAudioInstance? audioInstance)
    {
        lock (mInstanceLock)
        {
            Reader?.Dispose();
            Reader = null;

            Instance?.Dispose();
            Instance = null;

            mActiveSource = null;
        }
    }

    public DynamicSoundEffectInstance? LoadSong()
    {
        lock (mInstanceLock)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(mRealPath))
                {

                    if (Reader == null)
                    {
                        // Do we have this cached?
                        if (Globals.ContentManager.MusicPacks != null && Globals.ContentManager.MusicPacks.Contains(Path.GetFileName(mRealPath)))
                        {
                            // Read from cache, but close reader when we're done with it!
                            Reader = new VorbisReader(Globals.ContentManager.MusicPacks.GetAsset(Path.GetFileName(mRealPath)), true);
                        }
                        else if (mCreateStream != null)
                        {
                            Reader = new VorbisReader(mCreateStream(), true);
                        }
                        else
                        {
                            Reader = new VorbisReader(mRealPath);
                        }
                    }

                    if (Instance != null)
                    {
                        Instance.Dispose();
                        Instance = null;
                    }

                    Instance = new DynamicSoundEffectInstance(
                        Reader.SampleRate, Reader.Channels == 1 ? AudioChannels.Mono : AudioChannels.Stereo
                    );

                    Instance.BufferNeeded += Instance_BufferNeeded;
                    mActiveSource = this;
                    return Instance;

                }
            }
            catch (Exception exception)
            {
                ApplicationContext.Context.Value?.Logger.LogError(exception, $"Error loading '{mPath}'.");
                ChatboxMsg.AddMessage(
                    new ChatboxMsg(
                        $"{Strings.Errors.LoadFile.ToString(Strings.Words.LcaseSound)} [{mPath}]", new Color(0xBF, 0x0, 0x0), Enums.ChatMessageType.Error
                    )
                );
            }
        }
        mActiveSource = this;
        return null;
    }

    private static void EnsureBuffersFilled()
    {
        var buffers = 3;
        var samples = 44100;
        var updateRate = 10;

        while (Globals.IsRunning)
        {
            Thread.Sleep((int)(1000 / Math.Max(updateRate,1)));
            lock (mInstanceLock)
            {
                if (mActiveSource != null)
                {
                    var reader = mActiveSource.Reader;
                    var soundInstance = mActiveSource.Instance;

                    if (reader != null && soundInstance != null && !soundInstance.IsDisposed) {
                        float[] sampleBuffer = null;
                        while (soundInstance.PendingBufferCount < buffers)
                        {
                            if (sampleBuffer == null)
                                sampleBuffer = new float[samples];

                            var read = reader.ReadSamples(sampleBuffer, 0, sampleBuffer.Length);
                            if (read == 0)
                            {
                                reader.DecodedPosition = 0;
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

                            soundInstance.SubmitBuffer(dataBuffer, 0, read << 1);
                        }
                    }
                }
            }
        }
    }

    private void Instance_BufferNeeded(object sender, EventArgs e)
    {
        const float sampleMinimum = short.MinValue;
        const float sampleMaximum = short.MaxValue;
        const float sampleScale = 32767f;

        const int buffers = 3;
        const int samples = 44100;

        var reader = Reader;
        var soundInstance = Instance;

        if (reader == null || soundInstance is not { IsDisposed: false })
        {
            return;
        }

        float[]? sampleBuffer = null;
        while (soundInstance.PendingBufferCount < buffers)
        {
            sampleBuffer ??= new float[samples];

            var read = reader.ReadSamples(sampleBuffer, 0, sampleBuffer.Length);
            if (read == 0)
            {
                reader.DecodedPosition = 0;
                continue;
            }

            var byteCount = read << 1;
            var dataBuffer = new byte[byteCount];

            for (var sampleIndex = 0; sampleIndex < read; ++sampleIndex)
            {
                var sample = (short)Math.Clamp(sampleBuffer[sampleIndex] * sampleScale, sampleMinimum, sampleMaximum);
                Unsafe.As<byte, short>(ref dataBuffer[(sampleIndex << 1) + 0]) = sample;
            }

            soundInstance.SubmitBuffer(dataBuffer, 0, byteCount);
        }
    }

    ~MonoMusicSource()
    {
        ReleaseInstance(null);
    }
}
