namespace Intersect.GameObjects
{
    public struct EditorCursor
    {
        public string Sprite;
        public int XClickPoint;
        public int YClickPoint;

        /// <summary>
        /// Sprite name, X click-point and Y click-point for an editor map tool cursor.
        /// </summary>
        /// <param name="spriteName">Sprite file name (must end in .png) for an editor map tool cursor.</param>
        /// <param name="xClickPoint">Cursor X click-point for an editor map tool cursor.</param>
        /// <param name="yClickPoint">Cursor Y click-point for an editor map tool cursor.</param>
        public EditorCursor(string spriteName, int xClickPoint, int yClickPoint)
        {
            Sprite = spriteName;
            XClickPoint = xClickPoint;
            YClickPoint = yClickPoint;
        }
    }
}
