using System;
using System.Collections.Generic;

namespace Konves.IrrigationCaddy.Client
{
    internal static class Utility
    {
        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> collection, T element)
        {
            yield return element;
            foreach (T x in collection)
                yield return x;
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> collection, T element)
        {
            foreach (T x in collection)
                yield return x;
            yield return element;
        }

        public static string ToTimeStamp(this DateTime datetime)
        {
            return ((long)datetime.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds).ToString();
        }
    }
}
