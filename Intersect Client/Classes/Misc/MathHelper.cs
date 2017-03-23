namespace Intersect_Client.Classes.Misc
{
    public static class MathHelper
    {
        public static double Lerp(double value1, double value2, double amount)
        {
            return value1 + (value2 - value1) * amount;
        }
    }
}