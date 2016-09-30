using Moq;

namespace LoggerComponent.UnitTests
{
    public class TestLogAdaptor:IEventLogger
    {
        /// <summary>
        ///     To hook mock object for Unit Testing
        /// </summary>
        public static Mock<IEventLogger> MockLogger;


        public void LogMessage(string message, Severity severity)
        {
            //operations done on this mock object are verifiyable
            MockLogger?.Object.LogMessage(message, severity);
        }
    }
}
