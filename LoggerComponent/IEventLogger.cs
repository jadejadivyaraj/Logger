namespace LoggerComponent
{
    /// <summary>
    ///     Enum repesenting supported levels of severity
    /// </summary>
    public enum Severity
    {
        Info,
        Warning,
        Error
    }

    /// <summary>
    ///     The logger interface to Log the Message for a given severity
    /// </summary>
    public interface IEventLogger
    {
        void LogMessage(string message, Severity severity);
    }
}
