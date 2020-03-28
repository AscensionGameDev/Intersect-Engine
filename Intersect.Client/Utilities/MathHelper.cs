namespace Intersect.Client.Utilities
{

    public static class MathHelper
    {

        public static double Lerp(double value1, double value2, double amount)
        {
            return value1 + (value2 - value1) * amount;
        }

        public static double Clamp(double value, double min, double max)
        {
            return value < min ? min : value > max ? max : value;
        }

        public static float Clamp(float value, float min, float max)
        {
            return value < min ? min : value > max ? max : value;
        }

        public static int Clamp(int value, int min, int max)
        {
            return value < min ? min : value > max ? max : value;
        }

        public static long Clamp(long value, long min, long max)
        {
            return value < min ? min : value > max ? max : value;
        }

        public static short Clamp(short value, short min, short max)
        {
            return value < min ? min : value > max ? max : value;
        }

        public static byte Clamp(byte value, byte min, byte max)
        {
            return value < min ? min : value > max ? max : value;
        }

        public static uint Clamp(uint value, uint min, uint max)
        {
            return value < min ? min : value > max ? max : value;
        }

        public static ulong Clamp(ulong value, ulong min, ulong max)
        {
            return value < min ? min : value > max ? max : value;
        }

        public static ushort Clamp(ushort value, ushort min, ushort max)
        {
            return value < min ? min : value > max ? max : value;
        }

        public static sbyte Clamp(sbyte value, sbyte min, sbyte max)
        {
            return value < min ? min : value > max ? max : value;
        }

    }

}
