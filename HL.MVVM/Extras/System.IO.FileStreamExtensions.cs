using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.IO
{
    /// <summary>
    /// Provides extension methods for FileStream to handle CSV files and text writing.
    /// </summary>
    public static class FileStreamExtensions
    {
        /// <summary>
        /// Writes a CSV row to the specified FileStream using the provided data.
        /// </summary>
        /// <param name="stream">The FileStream to write the CSV row to.</param>
        /// <param name="data">An array of objects representing the values in the CSV row.</param>
        public static void WriteCSVRow(this FileStream stream, params object[] data)
        {
            string value = String.Empty;
            if (stream.Position > 0)
                stream.WriteText(Environment.NewLine);
            else
            {
                var bom = Encoding.UTF8.GetPreamble();
                stream.Write(bom, 0, bom.Length);
            }

            for (int x = 0; x < data.Length; x++)
            {
                if (!(data[x] is null))
                    value = data[x].ToString();
                else
                    value = "NULL";

                if (value.Contains(";") || value.Contains("\""))
                {
                    value = '"' + value.Replace("\"", "\"\"") + '"';
                }

                stream.WriteText(value);

                if (x != (data.Length - 1))
                {
                    stream.WriteText(";");
                }

            }
        }
        /// <summary>
        /// Writes a specified text to the FileStream using the given encoding (default is UTF8).
        /// </summary>
        /// <param name="stream">The FileStream to write the text to.</param>
        /// <param name="text">The text to be written to the FileStream.</param>
        /// <param name="encoding">The encoding to be used when writing the text. Defaults to UTF8 if not provided.</param>
        public static void WriteText(this FileStream stream, string text, Encoding encoding = null)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            stream.Write(bytes, 0, bytes.Length);
        }
    }
}
