using System;

namespace LoggerComponent.ExceptionParsers
{
    public interface IExceptionParserStrategy
    {
        /// <summary>
        ///     Parses the useful information or fields from the given exception based on implemented Strategy
        ///     We can have application level common parsing strategies eg GeneralParsingStrategy 
        ///     or Strategies specific to concrete exception types eg. InvalidOperationParserStrategy / AggregateExceptionParsingStrategy etc
        /// </summary>
        /// <param name="e">The exception that is to be parsed</param>
        /// <returns>String formed by parsing the exception</returns>
        string ParseException(Exception e);
    }
}
