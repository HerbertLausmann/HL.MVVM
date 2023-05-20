using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace HL.MVVM.Data
{
    /// <summary>
    /// A Class to help logging in a application in CSV format, more human readable
    /// </summary>
    public class CSVLog : DisposableBase
    {
        private string _filepath;
        private FileStream _FileStream;

        private int _TotalMessages;
        private int _TotalWarnings;
        private int _TotalErrors;
        private int _TotalFails;

        private DateTime _StartTime;
        private DateTime _EndTime;
        private TimeSpan _TimeElapsed;

        /// <summary>
        /// Gets the total messages logged.
        /// </summary>
        public int TotalMessages
        {
            get => _TotalMessages;
        }

        /// <summary>
        /// Gets the total warnings logged.
        /// </summary>
        public int TotalWarnings
        {
            get => _TotalWarnings;
        }

        /// <summary>
        /// Gets the total errors logged.
        /// </summary>
        public int TotalErrors
        {
            get => _TotalErrors;
        }

        /// <summary>
        /// Gets the total fails logged.
        /// </summary>
        public int TotalFails
        {
            get => _TotalFails;
        }

        /// <summary>
        /// Gets the file path of the CSV log.
        /// </summary>
        public string FilePath
        {
            get => _filepath;
        }

        /// <summary>
        /// Initializes a new instance of the CSVLog class.
        /// </summary>
        /// <param name="FilePath">The file path for the CSV log file.</param>
        public CSVLog(string FilePath)
        {
            Init(FilePath);
        }

        protected CSVLog()
        {

        }

        /// <summary>
        /// Writes a row to the CSV log file.
        /// </summary>
        /// <param name="data">An array of strings representing the row data.</param>
        protected void WriteRow(params string[] data)
        {
            lock (_FileStream)
            {
                _FileStream.WriteCSVRow(data);
                _FileStream.Flush();
            }
        }


        /// <summary>
        /// Initializes the CSV log with the specified file path.
        /// </summary>
        /// <param name="filePath">The file path for the CSV log file.</param>
        protected void Init(string filePath)
        {
            HL.MVVM.Extras.Helper.CleanUpLogFolder(filePath, 40);
            var dir = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            _filepath = filePath;
            _FileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            _StartTime = DateTime.Now;
            WriteRow("Timestamp", "Type", "Message");
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="Message">The message to log.</param>
        public virtual void Log(string Message)
        {
            WriteRow(DateTime.Now.ToShortTimeString(), "Message", Message);
            _TotalMessages += 1;
        }

        /// <summary>
        /// Logs a warning.
        /// </summary>
        /// <param name="Message">The warning message to log.</param>
        public virtual void Warning(string Message)
        {
            WriteRow(DateTime.Now.ToShortTimeString(), "⚠ Warning ⚠", Message);
            _TotalWarnings += 1;
        }

        /// <summary>
        /// Logs an error.
        /// </summary>
        /// <param name="Message">The error message to log.</param>
        public virtual void Error(string Message)
        {
            WriteRow(DateTime.Now.ToShortTimeString(), "❌ Error ❌", Message);
            _TotalErrors += 1;
        }

        /// <summary>
        /// Logs a fail.
        /// </summary>
        /// <param name="Message">The fail message to log.</param>
        public virtual void Fail(string Message)
        {
            WriteRow(DateTime.Now.ToShortTimeString(), "🚫 Fail 🚫", Message);
            _TotalFails += 1;
        }

        /// <summary>
        /// Logs a message of a specified message type.
        /// </summary>
        /// <param name="Message">The message to log.</param>
        /// <param name="MessageType">The message type.</param>
        public virtual void Log(string Message, MessageType MessageType)
        {
            switch (MessageType)
            {
                case MessageType.Message:
                    Log(Message);
                    break;
                case MessageType.Warning:
                    Warning(Message);
                    break;
                case MessageType.Fail:
                    Fail(Message);
                    break;
                case MessageType.Error:
                    Error(Message);
                    break;
                default:
                    break;
            }

        }
        /// <summary>
        /// Disposes of the CSVLog instance and writes summary information to the log file.
        /// </summary>
        protected override void DoDispose()
        {
            WriteRow(DateTime.Now.ToShortTimeString(), "END OF LOG", "END OF LOG");
            WriteRow(DateTime.Now.ToShortTimeString(), "TOTAL MESSAGES", _TotalMessages.ToString());
            WriteRow(DateTime.Now.ToShortTimeString(), "TOTAL WARNINGS", _TotalWarnings.ToString());
            WriteRow(DateTime.Now.ToShortTimeString(), "TOTAL ERRORS", _TotalErrors.ToString());
            WriteRow(DateTime.Now.ToShortTimeString(), "TOTAL FAILS", _TotalFails.ToString());
            _EndTime = DateTime.Now;
            _TimeElapsed = _EndTime - _StartTime;
            WriteRow(DateTime.Now.ToShortTimeString(), "TIME ELAPSED", _TimeElapsed.ToString(@"hh\:mm\:ss"));
            WriteRow(DateTime.Now.ToShortTimeString(), "END OF LOG", "END OF LOG");
            _FileStream.Close();
        }

        /// <summary>
        /// Enum representing different types of log messages.
        /// </summary>
        public enum MessageType : ushort
        {
            /// <summary>
            /// Represents an informational log message.
            /// </summary>
            Message = 200,

            /// <summary>
            /// Represents a log message indicating a potential issue or concern.
            /// </summary>
            Warning = 199,

            /// <summary>
            /// Represents a log message indicating a failed operation or task.
            /// </summary>
            Fail = 400,

            /// <summary>
            /// Represents a log message indicating an error or exception that has occurred.
            /// </summary>
            Error = 500
        }
    }
}
