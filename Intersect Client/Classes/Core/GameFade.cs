/*
    The MIT License (MIT)

    Copyright (c) 2015 JC Snider, Joe Bridges
  
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/

using Intersect_Client.Classes.General;

namespace Intersect_Client.Classes.Core
{
    public static class GameFade
    {
        private static FadeType _currentAction;
        private static float _fadeAmt;
        private static float _fadeRate = 3000f;
        private static long _lastUpdate = 0;

        public enum FadeType
        {
            None = 0,
            In = 1,
            Out = 2,
        }

        public static void FadeIn()
        {
            _currentAction = FadeType.In;
            _fadeAmt = 255f;
            _lastUpdate = Globals.System.GetTimeMS();
        }

        public static void FadeOut()
        {
            _currentAction = FadeType.Out;
            _fadeAmt = 0f;
            _lastUpdate = Globals.System.GetTimeMS();
        }

        public static bool DoneFading()
        {
            return (_currentAction == FadeType.None);
        }

        public static float GetFade()
        {
            return _fadeAmt;
        }

        public static void Update()
        {
            if (_currentAction == FadeType.In)
            {
                _fadeAmt -= ((Globals.System.GetTimeMS() - _lastUpdate) / _fadeRate) * 255f;
                if (_fadeAmt <= 0f)
                {
                    _currentAction = FadeType.None;
                    _fadeAmt = 0f;
                }
            }
            else if (_currentAction == FadeType.Out)
            {
                _fadeAmt += ((Globals.System.GetTimeMS() - _lastUpdate)/_fadeRate)*255f;
                if (_fadeAmt >= 255f)
                {
                    _currentAction = FadeType.None;
                    _fadeAmt = 255f;
                }
            }
            _lastUpdate = Globals.System.GetTimeMS();
        }
    }
}
