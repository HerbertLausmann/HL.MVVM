using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
    public static class ListExtensions
    {

        public static void CheckAdd(this IList list, bool check, object value)
        {
            if (check)
                list.Add(value);
        }
    }
}
