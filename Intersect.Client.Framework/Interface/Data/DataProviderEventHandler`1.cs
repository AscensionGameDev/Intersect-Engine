namespace Intersect.Client.Interface.Data;

public delegate void DataProviderEventHandler<in TEventArgs>(IDataProvider sender, TEventArgs args)
    where TEventArgs : EventArgs;