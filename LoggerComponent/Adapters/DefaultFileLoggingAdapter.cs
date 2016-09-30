using System;
using System.Collections.Concurrent;
using System.IO;
using LoggerComponent.Model;

namespace LoggerComponent.Adapters
{
    public class DefaultFileLoggingAdapter : AbstractLoggerAdapter
    {
        private bool _disposed;
        private StreamWriter _fileHandler;

        public DefaultFileLoggingAdapter() : base(new ConcurrentQueue<LogData>())
        {
            //the path or folder can be read from configuration or from constructor 
            
            //also not here we are passing the concurrentqueue to base class as we want to gaurantee order of removing items from collection 
            //but we can also pass other faster colection like concurrenBag if order is not important

            CloseOldFileIfExistsAndCreateNewFile();
        }

        protected override void WriteLog(LogData data)
        {
            if (data.TimeStamp.Date < DateTime.Today)
            {
                CloseOldFileIfExistsAndCreateNewFile();
            }
            _fileHandler.WriteLine(data);
        }

        private void CloseOldFileIfExistsAndCreateNewFile()
        {
            _fileHandler?.Dispose();
            _fileHandler = new StreamWriter(GetFileName(DateTime.Today), true);
            _fileHandler.AutoFlush = true;
        }

        private static string GetFileName(DateTime dateTime)
        {
            //hardcoded path here but can be retrieved from config or constructor
            return $@"C:\Temp\Log_{dateTime.Year}_{dateTime.Month}_{dateTime.Day}.log";
        }


        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _fileHandler?.Dispose();
                _fileHandler = null;
            }

            _disposed = true;

            base.Dispose(disposing);
        }
    }
}
