using System.Text;

namespace Intersect
{
    public static class SharedConstants
    {
        public static readonly string VERSION_NAME = "Beta Delphini";
        public static readonly byte[] VERSION_DATA = Encoding.UTF8.GetBytes(VERSION_NAME);
    }
}