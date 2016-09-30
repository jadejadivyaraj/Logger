using System;
using System.Globalization;

namespace LoggerComponent.Model
{
    /// <summary>
    ///     The Message Data Object
    /// </summary>
    public class LogData
    {
        /// <summary>
        ///     Time Stamp of the Log entry
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        ///     Process Id
        /// </summary>
        public int Process { get; set; }

        /// <summary>
        ///     Thread Id
        /// </summary>
        public int Thread { get; set; }

        /// <summary>
        ///     Severity of the Message
        /// </summary>
        public Severity Severity { get; set; }

        /// <summary>
        ///     The Message
        /// </summary>
        public string Message { get; set; }

        public override string ToString() => $"{TimeStamp.ToString(CultureInfo.InvariantCulture)};{Process};{Thread};{Severity};{Message}";
    }
}
