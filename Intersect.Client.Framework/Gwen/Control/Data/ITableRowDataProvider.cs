namespace Intersect.Client.Framework.Gwen.Control.Data
{
    public partial class RowDataChangedEventArgs : CellDataChangedEventArgs
    {
        public RowDataChangedEventArgs(int column, object oldValue, object newValue)
            : base(oldValue, newValue)
        {
            Column = column;
        }

        public int Column { get; }
    }

    public delegate void TableRowDataChangedEventHandler(object sender, RowDataChangedEventArgs args);

    public interface ITableRowDataProvider
    {
        event TableRowDataChangedEventHandler DataChanged;
    }
}
