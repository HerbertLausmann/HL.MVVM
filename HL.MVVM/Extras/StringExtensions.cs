using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class StringExtensions
    {

        public static string FirstName(this string str)
        {
            if (!string.IsNullOrWhiteSpace(str))
            {
                if (str.Contains(" "))
                {
                    return str.Split(' ')[0];
                }
                else
                    return str;
            }
            else
                return str;
        }

        public static string LastName(this string str)
        {
            if (!string.IsNullOrWhiteSpace(str))
            {
                if (str.Contains(" "))
                {
                    return string.Join(" ", str.Split(' ').Skip(1));
                }
                else
                    return str;
            }
            else
                return str;
        }

        public static string UseAsSeparator(this string str, string[] array)
        {
            string s = null;
            try
            {
                s = string.Join(str, array);
            }
            catch { }
            return s;
        }

    }
}
