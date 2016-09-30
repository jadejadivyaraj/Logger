using System;
using System.Configuration;
using System.Linq;
using LoggerComponent.Adapters;
using LoggerComponent.ExceptionParsers;

namespace LoggerComponent
{
    /// <summary>
    ///     The Logger Class that will be used by all application class to log
    /// </summary>
    public class Logger : IDisposable, IControlledShutDown
    {
        private static readonly object Lock = new object();
        private static Logger _instance;

        private static IEventLogger _adapter;
        private static IEventLogger _infoAdapter;
        private static IEventLogger _warningAdapter;
        private static IEventLogger _errorAdapter;

        private static IExceptionParserStrategy _exceptionParserStrategy;

        /// <summary>
        ///     Singleton pattern implementation of logger class
        /// </summary>
        /// <returns>Logger Instance</returns>
        public static Logger GetLogger()
        {
            lock (Lock)
            {
                if (_instance == null)
                {
                    _instance = new Logger();
                }
            }
            return _instance;
        }

        private Logger()
        {
            try
            {
                bool logErrors = false, logWarnings = false, logInfos = false;
                Type adapterType = null;
                //if configuration is  available
                if (ConfigurationManager.AppSettings.AllKeys.Any())
                {
                    //Reading from Configuration file messages of what all type of severity  to be logged

                    logErrors = bool.Parse(PargeConfiguration("LogError") ?? "false");

                    logWarnings = bool.Parse(PargeConfiguration("LogWarning") ?? "false");

                    logInfos = bool.Parse(PargeConfiguration("LogInformation") ?? "false");

                  
                    //create any adaptor as per the defined in configuration or the default logger.
                    var adaptyorType = PargeConfiguration("EventLoggerAdapterType");

                    if (string.IsNullOrEmpty(adaptyorType))
                    {
                        return;
                    }
                    adapterType = Type.GetType(adaptyorType, true);
                }

                if (!logInfos && !logErrors && !logWarnings)
                {
                    //mean we dont want to log any kind of error hence no need to create any logger adaptor
                    return;
                }

                IEventLogger instance = null;

                if (adapterType != null)
                {
                    instance = Activator.CreateInstance(adapterType) as IEventLogger;
                }

                _adapter = instance ?? new DefaultLoggingAdapter();

                //here using same intance of adaptor for all severity but we can even introduce logic here to read from config file,
                //the different adaptors for different severity
                //eg INFO logged in some file
                // WARNING logged Other File or  Console or Debug Window
                // ERROS logged in Console or DB or etc 

                if (logErrors)
                {
                    _errorAdapter = instance;
                }
                if (logWarnings)
                {
                    _warningAdapter = instance;
                }
                if (logInfos)
                {
                    _infoAdapter = instance;
                }

                //Hardcoding strategy here for now but this can also in Configuration if required
                _exceptionParserStrategy = new DefaultExceptionParsingStrategy();
            }
            catch (Exception e)
            {
                // added this catch to handle the case of possiblity of wrong configuration or any other exception.
            }
        }

        private static string PargeConfiguration(string key)
            => ConfigurationManager.AppSettings.AllKeys.Contains(key) ? ConfigurationManager.AppSettings[key] : null;


        /// <summary>
        ///     Log the message with given severity
        /// </summary>
        /// <param name="severity">Severity (<see cref="Severity" />) </param>
        /// <param name="message">the message that is to be logged</param>
        public static void LogMessage(string message, Severity severity) => _adapter?.LogMessage(message, severity);


        /// <summary>
        ///     Log Message with Severiy as Info (<see cref="Severity.Info" />)
        /// </summary>
        /// <param name="message">the message that is to be logged</param>
        public static void LogInfo(string message) => _infoAdapter?.LogMessage(message, Severity.Info);


        /// <summary>
        ///     Log Message with Severiy as Warning (<see cref="Severity.Warning" />)
        /// </summary>
        /// <param name="message">the message that is to be logged</param>
        public static void LogWarning(string message) => _warningAdapter?.LogMessage(message, Severity.Warning);


        /// <summary>
        ///     Log Message with Severiy as Error (<see cref="Severity.Error" />)
        /// </summary>
        /// <param name="message">the message that is to be logged</param>
        public static void LogError(string message) => _errorAdapter?.LogMessage(message, Severity.Error);


        /// <summary>
        ///     Log Message with Severiy as Error (<see cref="Severity.Error" />)
        /// </summary>
        /// <param name="ex">The Exception from which the message is to be parsed based on parsing Strategy</param>
        public static void LogError(Exception ex)
            => _errorAdapter?.LogMessage(FormatException(ex), Severity.Error);


        /// <summary>
        ///     Log Message with Severiy as Error (<see cref="Severity.Error" />)
        /// </summary>
        /// <param name="message">the message that is to be logged</param>
        /// <param name="ex">The Exception from which the message is to be parsed based on parsing Strategy</param>
        public static void LogError(string message, Exception ex)
            => _errorAdapter?.LogMessage(ex == null ? message : FormatException(ex, message), Severity.Error);


        private static string FormatException(Exception ex, string message = null)
        {
            if (!string.IsNullOrEmpty(message))
            {
                return message + _exceptionParserStrategy?.ParseException(ex);
            }

            return _exceptionParserStrategy?.ParseException(ex);
        }

        /// <summary>
        ///     Disposes the Object after finishing the pending tasks
        /// </summary>
        public void Dispose()
        {
            ShutDown();
        }

        /// <summary>
        ///     Shutdown the Logger
        /// </summary>
        /// <param name="forceShutDown">
        ///     if false will wait untill pending jobs are finished & then shutdown
        ///     if true forces shutdown ignoring pending jobs
        /// </param>
        public void ShutDown(bool forceShutDown = false)
        {
            var adapter = _adapter as IControlledShutDown;
            _adapter = _errorAdapter = _warningAdapter = _infoAdapter = null;
            _instance = null;
            adapter?.ShutDown(forceShutDown);
        }
    }
}
