using System;
using System.Collections.Generic;

namespace PieChart.Models
{
    public static class StaticClass
    {
        public static IEnumerable<TSource> CustomDistinct<TSource, TKey>
    (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}
