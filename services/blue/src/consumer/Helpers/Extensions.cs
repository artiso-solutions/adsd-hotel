using System;
using System.Collections.Generic;
using System.Linq;

namespace artiso.AdsdHotel.Blue.Consumer
{
    internal static class Extensions
    {
        public static T PickRandom<T>(this IReadOnlyList<T> list)
        {
            if (!list.Any()) throw new Exception("Cannot pick a random item from an empty list");
            var index = new Random().Next(0, list.Count);
            return list[index];
        }
    }
}
