namespace Intersect.Client.Framework.GenericClasses;


public partial class Cursor
{

    private int mType = 0;

    public Cursor()
    {
    }

    public Cursor(Cursors.CursorTypes t)
    {
        mType = (int) t;
    }

    /// <summary>
    /// The kind of cursor this represents - used by the client to pick which custom cursor texture (if
    /// any) to draw in place of the OS cursor.
    /// </summary>
    public Cursors.CursorTypes Type => (Cursors.CursorTypes) mType;

    public static Cursor Current { get; set; }

}

public static partial class Cursors
{

    public enum CursorTypes
    {

        Default = 0, //default

        SizeAll,

        SizeNs,

        SizeWe,

        SizeNwse,

        SizeNesw,

        No,

        Hand,

        Drag

    }

    public static Cursor Default => new Cursor(CursorTypes.Default);

    public static Cursor SizeNs => new Cursor(CursorTypes.SizeNs);

    public static Cursor SizeWe => new Cursor(CursorTypes.SizeWe);

    public static Cursor SizeAll => new Cursor(CursorTypes.SizeAll);

    public static Cursor SizeNwse => new Cursor(CursorTypes.SizeNwse);

    public static Cursor SizeNesw => new Cursor(CursorTypes.SizeNesw);

    public static Cursor No => new Cursor(CursorTypes.No);

    public static Cursor Hand => new Cursor(CursorTypes.Hand);

    public static Cursor Drag => new Cursor(CursorTypes.Drag);

}
