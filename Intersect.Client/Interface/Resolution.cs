namespace Intersect.Client.Interface
{

    public struct Resolution
    {

        public ushort X;

        public ushort Y;

        public Resolution(long x = 800, long y = 600)
        {
            X = (ushort) (x & 0xFFFF);
            Y = (ushort) (y & 0xFFFF);
        }

        private Resolution(string resolution) : this()
        {
            ushort x;
            ushort y;
            var split = resolution?.Split('x', ',', ' ', '/', '-', '_', '.', '~');
            if (ushort.TryParse(split?[0], out x))
            {
                X = x;
            }

            if (ushort.TryParse(split?[1], out y))
            {
                Y = y;
            }
        }

        public static Resolution Parse(string resolution)
        {
            return new Resolution(resolution);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Resolution))
            {
                return false;
            }

            var resolution = (Resolution) obj;

            return resolution.X == X && resolution.Y == Y;
        }

        public bool Equals(Resolution other)
        {
            return X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
        {
            return (X << 16) & Y;
        }

        public override string ToString()
        {
            return $"{X},{Y}";
        }

    }

}
