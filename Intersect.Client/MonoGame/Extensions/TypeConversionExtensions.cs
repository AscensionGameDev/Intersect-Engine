using Intersect.Client.Framework.GenericClasses;

using Microsoft.Xna.Framework;

namespace Intersect.Client.MonoGame.Extensions
{
    public static class TypeConversionExtensions
    {
        public static Vector2 AsVector2(this Pointf point) => new Vector2(point.X, point.Y);

        public static Vector4 AsVector4(this Color color) => new Vector4(
            color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f
        );

        public static Color AsIntersectColor(this Microsoft.Xna.Framework.Color color) =>
            new Color(color.A, color.R, color.G, color.B);

        public static Microsoft.Xna.Framework.Color AsMonoColor(this Color color) =>
            new Microsoft.Xna.Framework.Color(color.R, color.G, color.B, color.A);
    }
}
