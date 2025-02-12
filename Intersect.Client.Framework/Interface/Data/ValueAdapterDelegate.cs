namespace Intersect.Client.Interface.Data;

public delegate TDependentValue ValueAdapterDelegate<in TBaseValue, out TDependentValue>(TBaseValue value, TBaseValue oldValue);