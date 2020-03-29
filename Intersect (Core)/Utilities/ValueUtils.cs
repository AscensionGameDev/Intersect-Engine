namespace Intersect.Utilities
{

    public static class ValueUtils
    {

        public static void Swap<T>(ref T a, ref T b)
        {
            var temp = a;
            a = b;
            b = temp;
        }

        public static bool SetDefault<T>(bool condition, out T value)
        {
            value = default(T);

            return condition;
        }

    }

}
