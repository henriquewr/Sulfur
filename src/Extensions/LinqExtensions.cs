using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sulfur.Extensions
{
    public static class LinqExtensions
    {
        public static T Pop<T>(this IList<T> values, int index = 0)
        {
            var value = values[index];
            values.RemoveAt(index);
            return value;
        }
    }
}
