namespace Intersect.Client.Framework.Graphics
{

    public abstract class GameFont
    {

        private string mFontName = @"";

        private int mFontSize = 12;

        public GameFont(string fontName, int fontSize)
        {
            mFontName = fontName;
            mFontSize = fontSize;
        }

        public string GetName()
        {
            return mFontName;
        }

        public int GetSize()
        {
            return mFontSize;
        }

        public abstract object GetFont();

        public static string ToString(GameFont font)
        {
            if (font == null)
            {
                return "";
            }

            return font.GetName() + "," + font.GetSize();
        }

    }

}
