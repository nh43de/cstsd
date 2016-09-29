using System;
using System.Collections.Generic;

namespace cstsd
{
    public static class EnumerableExtensions
    {

        public static void For<T>(this IEnumerable<T> items, Action<T> itemAction)
        {
            foreach (var item in items)
            {
                itemAction(item);
            }
        }



    }
}