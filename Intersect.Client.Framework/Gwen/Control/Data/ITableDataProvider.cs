namespace Intersect.Client.Framework.Gwen.Control.Data
{
    public partial class TableDataChangedEventArgs : RowDataChangedEventArgs
    {
        public TableDataChangedEventArgs(int row, int column, object oldValue, object newValue)
            : base(column, oldValue, newValue)
        {
            Row = row;
        }

        public int Row { get; }
    }

    public delegate void TableDataChangedEventHandler(object sender, TableDataChangedEventArgs args);

    public interface ITableDataProvider
    {
        event TableDataChangedEventHandler DataChanged;
    }
}
