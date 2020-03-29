namespace Intersect.Client.Framework.GenericClasses
{

    public class Cursor
    {

        private int mType = 0;

        public Cursor()
        {
        }

        public Cursor(Cursors.CursorTypes t)
        {
            mType = (int) t;
        }

        public static Cursor Current { get; set; }

    }

    public static class Cursors
    {

        public enum CursorTypes
        {

            Default = 0, //default

            SizeAll,

            SizeNs,

            SizeWe,

            SizeNwse,

            SizeNesw,

            No

        }

        public static Cursor Default => new Cursor(CursorTypes.Default);

        public static Cursor SizeNs => new Cursor(CursorTypes.SizeNs);

        public static Cursor SizeWe => new Cursor(CursorTypes.SizeWe);

        public static Cursor SizeAll => new Cursor(CursorTypes.SizeAll);

        public static Cursor SizeNwse => new Cursor(CursorTypes.SizeNwse);

        public static Cursor SizeNesw => new Cursor(CursorTypes.SizeNesw);

        public static Cursor No => new Cursor(CursorTypes.No);

    }

}
