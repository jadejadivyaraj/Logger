using System;

namespace LoggerComponent.ExceptionParsers
{
    /// <summary>
    ///         Default Strategy that return the exception Message
    /// </summary>
    class DefaultExceptionParsingStrategy :IExceptionParserStrategy
    {
        /// <summary>
        ///     Parses the given exception & returns string
        /// </summary>
        /// <param name="e">The exception that is to be parsed</param>
        /// <returns>String formed by parsing the exception</returns>
        public string ParseException(Exception e)
        {
            //this is default strategy hence just parsing message filed.
            // dependingon the requirement we can parse inner exception, stack trace or any of the details
            return e.Message;
        }
    }
}
