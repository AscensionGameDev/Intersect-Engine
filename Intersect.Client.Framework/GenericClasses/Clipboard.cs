namespace Intersect.Client.Framework.GenericClasses
{

    public static class Clipboard
    {

        private static string sValue = null;

        public static bool ContainsText()
        {
            return sValue != null;
        }

        public static string GetText()
        {
            if (ContainsText())
            {
                return sValue;
            }

            return "";
        }

        public static void SetText(string text)
        {
            sValue = text;
        }

    }

}
