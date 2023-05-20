using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL.MVVM.Extras
{
    public static class Helper
    {
        public static int? TryParseStringToInt(string str)
        {
            int v = -1;
            var t = int.TryParse(str, out v);
            if (!t) return null;
            return v;
        }

        public static int TryParseStringToInt2(string str)
        {
            int v = -1;
            var t = int.TryParse(str, out v);
            if (!t) return -1;
            return v;
        }

        public static string GetNumbers(string str)
        {
            try
            {
                string n = new String(str.Where(Char.IsDigit).ToArray());
                return n;
            }
            catch
            {
                return null;
            }
        }

        public static bool IsRecent30Days(string date)
        {
            DateTime dt = DateTime.MinValue;
            DateTime.TryParse(date, out dt);

            var diff = DateTime.Today.Subtract(dt);

            return diff.TotalDays <= 30;
        }

        public static void CleanUpLogFolder(string path, int maxDays)
        {
            try
            {
                Directory.GetFiles(Path.GetDirectoryName(path))
                         .Select(f => new FileInfo(f))
                         .Where(f => f.LastAccessTime < DateTime.Now.AddDays(-maxDays))
                         .ToList()
                         .ForEach(f => f.Delete());
            }
            catch
            {
            }
        }

    }
}
