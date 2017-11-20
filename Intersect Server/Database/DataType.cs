using System;

namespace Intersect.Server.Database
{
    public enum DataType
    {
        Blob = 0,
        Integer,
        Number,
        Text
    }

    public static class DataTypeExtensions
    {
        public static string ToSql(this DataType dataType)
        {
            switch (dataType)
            {
                case DataType.Number:
                    return "NUMERIC";

                case DataType.Blob:
                case DataType.Integer:
                case DataType.Text:
                    return dataType.ToString().ToUpper();

                default:
                    throw new ArgumentException(@"Invalid dataType used in ToSql", nameof(dataType));
            }
        }
    }
}