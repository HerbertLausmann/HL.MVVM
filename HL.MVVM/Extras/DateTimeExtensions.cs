using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    internal static class DateTimeExtensions
    {
        public static bool IsOlderThan(this DateTime date, double days)
        {
            DateTime currentDate = DateTime.Now;

            TimeSpan difference = currentDate - date;

            if (difference.TotalDays > days)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
