using System.Collections.Generic;

using Newtonsoft.Json;

namespace Intersect.Utilities
{

    public static partial class DatabaseUtils
    {

        public static int[] LoadIntArray(string json, int arrayLen)
        {
            var output = new int[arrayLen];
            var jsonList = new List<int>();
            if (json != null)
            {
                jsonList = JsonConvert.DeserializeObject<List<int>>(json);
            }

            for (var i = 0; i < arrayLen && i < jsonList.Count; i++)
            {
                output[i] = jsonList[i];
            }

            return output;
        }

        public static void LoadIntArray(ref int[] output, string json, int arrayLen)
        {
            var jsonList = JsonConvert.DeserializeObject<List<int>>(json);
            for (var i = 0; i < arrayLen && i < jsonList.Count; i++)
            {
                output[i] = jsonList[i];
            }
        }

        public static string SaveIntArray(int[] array, int arrayLen)
        {
            if (array == null)
            {
                array = new int[arrayLen];
            }

            var output = new List<int>();
            for (var i = 0; i < arrayLen; i++)
            {
                if (i < array.Length)
                {
                    output.Add(array[i]);
                }
                else
                {
                    output.Add(0);
                }
            }

            return JsonConvert.SerializeObject(output);
        }

        public static long[] LoadLongArray(string json, long arrayLen)
        {
            var output = new long[arrayLen];
            var jsonList = json != null ? JsonConvert.DeserializeObject<List<long>>(json) : new List<long>();

            for (var i = 0; i < arrayLen && i < jsonList.Count; i++)
            {
                output[i] = jsonList[i];
            }

            return output;
        }

        public static void LoadLongArray(ref long[] output, string json, long arrayLen)
        {
            var jsonList = JsonConvert.DeserializeObject<List<long>>(json);

            for (var i = 0; i < arrayLen && i < jsonList.Count; i++)
            {
                output[i] = jsonList[i];
            }
        }

        public static string SaveLongArray(long[] array, long arrayLen)
        {
            array ??= new long[arrayLen];

            var output = new List<long>();
            for (var i = 0; i < arrayLen; i++)
            {
                output.Add(i < array.Length ? array[i] : 0);
            }

            return JsonConvert.SerializeObject(output);
        }

        public static string SaveColor(Color color)
        {
            if (color == null)
            {
                color = new Color();
            }

            return JsonConvert.SerializeObject(color);
        }

        public static Color LoadColor(string json)
        {
            var color = new Color();
            if (json != null)
            {
                color = JsonConvert.DeserializeObject<Color>(json);
            }

            return color;
        }

    }

}
