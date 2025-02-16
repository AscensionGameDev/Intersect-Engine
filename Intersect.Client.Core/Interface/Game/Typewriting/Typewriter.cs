using Intersect.Client.Core;
using Intersect.Client.Utilities;
using Intersect.Configuration;
using Intersect.Framework.Core;
using Intersect.Utilities;

namespace Intersect.Client.Interface.Game.Typewriting;

internal sealed class Typewriter
{
    public delegate void TextWrittenHandler(string text, Color color);
    private static HashSet<string> FullStops => ClientConfiguration.Instance.TypewriterFullStops;
    private static long FullStopSpeed => ClientConfiguration.Instance.TypewriterFullStopDelay;
    private static HashSet<string> PartialStops => ClientConfiguration.Instance.TypewriterPauses;
    private static long PartialStopSpeed => ClientConfiguration.Instance.TypewriterPauseDelay;
    private static long TypingSpeed => ClientConfiguration.Instance.TypewriterPartDelay;

    private int _offset;
    private int _segmentIndex;
    private string? _lastText;
    private long _nextUpdateTime;

    private readonly TextWrittenHandler _textWrittenHandler;
    private readonly ColorizedText[] _segments;

    public bool IsDone { get; private set; }
    public long DoneAtMilliseconds { get; private set; }

    public Typewriter(ColorizedText[] segments, TextWrittenHandler textWrittenHandler)
    {
        _segments = segments;
        _textWrittenHandler = textWrittenHandler;
        _nextUpdateTime = Timing.Global.MillisecondsUtc;
        _segmentIndex = 0;
        _offset = 0;
        _lastText = null;
        IsDone = false;
    }

    public void Write(string? soundName)
    {
        if (IsDone)
        {
            return;
        }

        if (_segmentIndex >= _segments.Length)
        {
            End();
            return;
        }

        if (Timing.Global.MillisecondsUtc < _nextUpdateTime)
        {
            return;
        }

        var emitSound = false;
        while (_nextUpdateTime <= Timing.Global.MillisecondsUtc)
        {
            if (_segmentIndex >= _segments.Length)
            {
                End();
                return;
            }

            emitSound |= _offset % ClientConfiguration.Instance.TypewriterSoundFrequency == 0;

            var segment = _segments[_segmentIndex];

            if (_offset >= segment.Text.Length)
            {
                _offset = 0;
                _segmentIndex++;

                if (_segmentIndex >= _segments.Length)
                {
                    End();
                    continue;
                }

                segment = _segments[_segmentIndex];
            }

            string nextText;
            if (char.IsSurrogatePair(segment.Text, _offset))
            {
                nextText = segment.Text[_offset..(_offset + 2)];
                _offset += 2;
            }
            else
            {
                nextText = segment.Text[_offset..(_offset + 1)];
                ++_offset;
            }

            _nextUpdateTime += GetTypingDelayFor(nextText, _lastText);
            _textWrittenHandler(nextText, segment.Color);
            _lastText = nextText;
        }

        if (!emitSound || string.IsNullOrEmpty(soundName))
        {
            return;
        }

        Audio.StopAllGameSoundsOf(ClientConfiguration.Instance.TypewriterSounds.ToArray());
        Audio.AddGameSound(soundName, false);
    }

    private static long GetTypingDelayFor(string next, string? last)
    {
        if (last == null)
        {
            return TypingSpeed;
        }

        if (next == "\n")
        {
            return FullStopSpeed;
        }

        if (next == last)
        {
            return TypingSpeed;
        }

        if (FullStops.Contains(last))
        {
            return FullStopSpeed;
        }

        return PartialStops.Contains(last) ? PartialStopSpeed : TypingSpeed;
    }

    public void End()
    {
        if (IsDone)
        {
            return;
        }

        while (_segmentIndex < _segments.Length)
        {
            var segment = _segments[_segmentIndex];
            if (_offset < segment.Text.Length)
            {
                _textWrittenHandler(segment.Text[_offset..], segment.Color);
            }
            _segmentIndex++;
            _offset = 0;
        }

        IsDone = true;
        DoneAtMilliseconds = Timing.Global.MillisecondsUtc;
    }
}
