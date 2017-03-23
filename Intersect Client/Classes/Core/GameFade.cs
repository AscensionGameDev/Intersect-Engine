using Intersect_Client.Classes.General;

namespace Intersect_Client.Classes.Core
{
    public static class GameFade
    {
        public enum FadeType
        {
            None = 0,
            In = 1,
            Out = 2,
        }

        private static FadeType _currentAction;
        private static float _fadeAmt;
        private static float _fadeRate = 3000f;
        private static long _lastUpdate = 0;

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
                _fadeAmt += ((Globals.System.GetTimeMS() - _lastUpdate) / _fadeRate) * 255f;
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