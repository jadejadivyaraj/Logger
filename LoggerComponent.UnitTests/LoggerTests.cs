using System;
using Moq;
using NUnit.Framework;

namespace LoggerComponent.UnitTests
{
    [TestFixture]
    public class LoggerTests
    {
        private Logger _logger;

        [SetUp]
        protected void SetUp()
        {

        }

        [TearDown]
        protected void TearDown()
        {
            _logger?.Dispose();
            _logger = null;
            TestLogAdaptor.MockLogger = null;
        }

        [Test]
        public void GetLogger_WhenCalled_InitializesAndReturnSingletonInstance()
        {
            _logger = Logger.GetLogger();

            var logger = Logger.GetLogger();

            Assert.IsTrue(ReferenceEquals(_logger, logger), "Singleton Pattern is broken");
        }


        [Test]
        public void LogError_WhenCalledWithoutInitializingLogger_DoesntLogAnythingButMustNotHaveAnyException()
        {
            Logger.LogError(" test log message");

        }


        [Test]
        public void LogErrorExceptionOverload_WhenCalledWithoutInitializingLogger_DoesntLogAnythingButMustNotHaveAnyException()
        {
            Logger.LogError(new InvalidOperationException("Some invalid operation message"));

        }

        [Test]
        public void LogErrorExceptionAndMessageOverload_WhenCalledWithoutInitializingLogger_DoesntLogAnythingButMustNotHaveAnyException()
        {
            Logger.LogError("Some custom message",new InvalidOperationException("Some invalid operation message"));

        }


        [Test]
        public void LogWarning_WhenCalledWithoutInitializingLogger_DoesntLogAnythingButMustNotHaveAnyException()
        {
            Logger.LogWarning(" test log message");

        }

        [Test]
        public void LogInfo_WhenCalledWithoutInitializingLogger_DoesntLogAnythingButMustNotHaveAnyException()
        {
            Logger.LogInfo(" test log message");
        }

        [Test]
        public void LogMessage_WhenCalledWithoutInitializingLogger_DoesntLogAnythingButMustNotHaveAnyException()
        {
            Logger.LogMessage(" test log message",Severity.Warning);
        }


        [Test]
        public void LogInfo_WhenCalledAfterInitializingLoggerAndHasConfigurationSetforInfo_MustLog()
        {
            var mockLogger = new Mock<IEventLogger>();
            var message = "test LogInfo message";
            mockLogger.Setup(x => x.LogMessage(message, Severity.Info)).Verifiable();
            TestLogAdaptor.MockLogger= mockLogger;
            Logger.GetLogger();


            Logger.LogInfo(message);


            mockLogger.Verify(x => x.LogMessage(message, Severity.Info), Times.Once);

        }

        [Test]
        public void LogWarning_WhenCalledAfterInitializingLoggerAndHasConfigurationSetforInfo_MustLog()
        {
            var mockLogger = new Mock<IEventLogger>();
            var message = "test LogWarning message";
            mockLogger.Setup(x => x.LogMessage(message, Severity.Warning)).Verifiable();
            TestLogAdaptor.MockLogger = mockLogger;
            Logger.GetLogger();


            Logger.LogWarning(message);


           
            mockLogger.Verify(x => x.LogMessage(message, Severity.Warning), Times.Once);


        }

        [Test]
        public void LogError_WhenCalledAfterInitializingLoggerAndHasConfigurationSetforInfo_MustLog()
        {
            var mockLogger = new Mock<IEventLogger>();
            var message = "test LogError message";
            mockLogger.Setup(x => x.LogMessage(message, Severity.Error)).Verifiable();
            TestLogAdaptor.MockLogger = mockLogger;
            Logger.GetLogger();


            Logger.LogError(message);


            mockLogger.Verify(x => x.LogMessage(message, Severity.Error), Times.Once);

        }

        [Test]
        public void LogErrorExceptionOverload_WhenCalledAfterInitializingLoggerAndHasConfigurationSetforInfo_MustLog()
        {
            var mockLogger = new Mock<IEventLogger>();
            var message = "test LogError message";
            var exception = new InvalidOperationException(message);
            mockLogger.Setup(x => x.LogMessage(message, Severity.Error)).Verifiable();
            TestLogAdaptor.MockLogger = mockLogger;
            Logger.GetLogger();


            Logger.LogError(exception);


            mockLogger.Verify(x => x.LogMessage(message, Severity.Error), Times.Once);

        }

        [Test]
        public void LogErrorExceptionAndMessageOverload_WhenCalledAfterInitializingLoggerAndHasConfigurationSetforInfo_MustLog()
        {
            var mockLogger = new Mock<IEventLogger>();
            var message1 = "a custom message";
            var message = "test LogError message";
            var exception = new InvalidOperationException(message);
            mockLogger.Setup(x => x.LogMessage(message1+message, Severity.Error)).Verifiable();
            TestLogAdaptor.MockLogger = mockLogger;
            Logger.GetLogger();


            Logger.LogError(message1,exception);


            mockLogger.Verify(x => x.LogMessage(message1+message, Severity.Error), Times.Once);

        }

        [Test]
        public void LogMessage_WhenCalledAfterInitializingLoggerAndHasConfigurationSetforInfo_MustLog()
        {
            var mockLogger = new Mock<IEventLogger>();
            var message = "test LogMessage with errorSev message";
            mockLogger.Setup(x => x.LogMessage(message, Severity.Error)).Verifiable();
            TestLogAdaptor.MockLogger = mockLogger;
            Logger.GetLogger();


            Logger.LogMessage(message, Severity.Error);


            mockLogger.Verify(x => x.LogMessage(message, Severity.Error), Times.Once);
        }

        [Test]
        public void LogError_WhenCalledAfterDisposingLogger_DoesntLogAnythingButMustNotHaveAnyException()
        {
            var mockLogger = new Mock<IEventLogger>();
            var message = "test LogMessage with errorSev message";
            mockLogger.Setup(x => x.LogMessage(message, Severity.Error)).Verifiable();
            TestLogAdaptor.MockLogger = mockLogger;
            (Logger.GetLogger() as IDisposable).Dispose();



            Logger.LogError(message);

            mockLogger.Verify(x=>x.LogMessage(message, Severity.Error),Times.Never);

        }

        [Test]
        public void LogErrorExceptionOverload_WhenCalledAfterDisposingLogger_DoesntLogAnythingButMustNotHaveAnyException()
        {
            var mockLogger = new Mock<IEventLogger>();
            var message = "test LogMessage with errorSev message";
            mockLogger.Setup(x => x.LogMessage(message, Severity.Error)).Verifiable();
            TestLogAdaptor.MockLogger = mockLogger;
            (Logger.GetLogger() as IDisposable).Dispose();



            Logger.LogError(new InvalidOperationException(message));

            mockLogger.Verify(x => x.LogMessage(message, Severity.Error), Times.Never);

        }

        [Test]
        public void LogErrorExceptionAndMessageOverload_WhenCalledAfterDisposingLogger_DoesntLogAnythingButMustNotHaveAnyException()
        {
            var mockLogger = new Mock<IEventLogger>();
            var message1 = "abcd";
            var message = "test LogMessage with errorSev message";
            mockLogger.Setup(x => x.LogMessage(message1+message, Severity.Error)).Verifiable();
            TestLogAdaptor.MockLogger = mockLogger;
            (Logger.GetLogger() as IDisposable).Dispose();



            Logger.LogError(message1,new InvalidOperationException(message));

            mockLogger.Verify(x => x.LogMessage(message1+message, Severity.Error), Times.Never);

        }

        [Test]
        public void LogWarning_WhenCalledAfterDisposingLogger_DoesntLogAnythingButMustNotHaveAnyException()
        {
            var mockLogger = new Mock<IEventLogger>();
            var message = "test LogWarning with warnSeverity message";
            mockLogger.Setup(x => x.LogMessage(message, Severity.Warning)).Verifiable();
            TestLogAdaptor.MockLogger = mockLogger;
            (Logger.GetLogger() as IDisposable).Dispose();



            Logger.LogWarning(message);

            mockLogger.Verify(x => x.LogMessage(message, Severity.Warning), Times.Never);

        }


        [Test]
        public void LogInfo_WhenCalledAfterDisposingLogger_DoesntLogAnythingButMustNotHaveAnyException()
        {
            var mockLogger = new Mock<IEventLogger>();
            var message = "test LogInfo with info message";
            mockLogger.Setup(x => x.LogMessage(message, Severity.Info)).Verifiable();
            TestLogAdaptor.MockLogger = mockLogger;
            (Logger.GetLogger() as IDisposable).Dispose();



            Logger.LogInfo(message);

            mockLogger.Verify(x => x.LogMessage(message, Severity.Info), Times.Never);

        }

        [Test]
        public void LogMessage_WhenCalledAfterDisposingLogger_DoesntLogAnythingButMustNotHaveAnyException()
        {
            var mockLogger = new Mock<IEventLogger>();
            var message = "test LogMessage with infoSverity  message";
            mockLogger.Setup(x => x.LogMessage(message, Severity.Info)).Verifiable();
            TestLogAdaptor.MockLogger = mockLogger;
            (Logger.GetLogger() as IDisposable).Dispose();



            Logger.LogMessage(message,Severity.Info);

            mockLogger.Verify(x => x.LogMessage(message, Severity.Info), Times.Never);

        }



    }
}
