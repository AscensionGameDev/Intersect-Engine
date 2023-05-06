using System.Collections.Generic;
using System.Linq;
using Intersect.Client.Core;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Configuration;
using Intersect.Utilities;

namespace Intersect.Client.Interface.Game.Typewriting
{
    internal sealed class Typewriter
    {
        private static List<char> _fullStopChars => ClientConfiguration.Instance.TypewriterFullStopCharacters;
        private static long _fullStopSpeed => ClientConfiguration.Instance.TypewriterFullStopDelay;
        private static List<char> _partialStopChars => ClientConfiguration.Instance.TypewriterPauseCharacters;
        private static long _partialStopSpeed => ClientConfiguration.Instance.TypewriterPauseDelay;
        private static long _typingSpeed => ClientConfiguration.Instance.TypewriterPartDelay;

        private List<Label> _labels;
        private string[] _lines;
        private int _lineCount;
        private int _lineIndex;
        private int _charIndex;
        private char? _lastChar;
        private long _nextUpdateTime;

        public bool IsDone { get; private set; }
        public long DoneAtMilliseconds { get; private set; }

        public void Initialize(List<Label> labels)
        {
            _nextUpdateTime = Timing.Global.MillisecondsUtc;
            _labels = labels;
            _lines = _labels.Select(l => l.Text).ToArray();
            _labels.ForEach(l =>
                l.SetText(string.Empty)); // Clear the text from the labels. The writer is the captain now
            _lineIndex = 0;
            _charIndex = 0;
            _lastChar = null;
            _lineCount = _lines.Length;
            IsDone = false;
        }

        private void NewLine()
        {
            _lineIndex++;
            if (_lineIndex >= _lineCount)
            {
                End();
                return;
            }

            _charIndex = 0;
            _lastChar = null;
        }

        public void Write(string voice)
        {
            if (IsDone)
            {
                return;
            }

            if (_lineIndex >= _lineCount || _labels[_lineIndex] == null)
            {
                End();
                return;
            }

            if (Timing.Global.MillisecondsUtc < _nextUpdateTime)
            {
                return;
            }

            if (!string.IsNullOrEmpty(voice) && _charIndex % ClientConfiguration.Instance.TypewriterSoundFrequency == 0)
            {
                Audio.StopAllGameSoundsOf(ClientConfiguration.Instance.TypewriterSounds.ToArray());
                Audio.AddGameSound(voice ?? string.Empty, false);
            }

            var currentLine = _lines[_lineIndex];
            _charIndex++;

            if (_charIndex >= currentLine.Length)
            {
                _labels[_lineIndex].SetText(currentLine);
                NewLine();
                return;
            }

            _lastChar = currentLine[_charIndex - 1];
            var written = currentLine.Substring(0, _charIndex);
            _labels[_lineIndex].SetText(written);

            _nextUpdateTime = Timing.Global.MillisecondsUtc + GetTypingDelayFor(currentLine[_charIndex], _lastChar);
        }

        private static long GetTypingDelayFor(char currentChar, char? lastChar)
        {
            if (lastChar == null)
            {
                return _typingSpeed;
            }

            var lastCharVal = lastChar.Value;

            if (currentChar == lastCharVal)
            {
                return _typingSpeed;
            }

            if (_fullStopChars.Contains(lastCharVal))
            {
                return _fullStopSpeed;
            }

            if (_partialStopChars.Contains(lastCharVal))
            {
                return _partialStopSpeed;
            }

            return _typingSpeed;
        }

        public void End()
        {
            if (IsDone || (_lines?.Length ?? 0) == 0)
            {
                return;
            }

            for (var i = 0; i < _lineCount; i++)
            {
                if (i >= _labels.Count)
                {
                    continue;
                }

                _labels[i].SetText(_lines[i]);
            }

            IsDone = true;
            DoneAtMilliseconds = Timing.Global.MillisecondsUtc;
        }
    }
}
