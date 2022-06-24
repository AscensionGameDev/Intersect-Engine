using System;

namespace Intersect.Client.Framework.Gwen.Control.Data
{
    public partial class CellDataChangedEventArgs : EventArgs
    {
        public CellDataChangedEventArgs(object oldValue, object newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        public object OldValue { get; }

        public object NewValue { get; }
    }

    public delegate void TableCellDataChangedEventHandler(object sender, CellDataChangedEventArgs args);

    public interface ITableCellDataProvider
    {
        event TableCellDataChangedEventHandler DataChanged;
    }
}
