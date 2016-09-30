using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using LoggerComponent.Model;

namespace LoggerComponent.Adapters
{
    /// <summary>
    ///     All Logger Adaptors derived from this gaurantees Fast return to calling method.
    ///     Provides Scalable Threadsafe Logging option
    ///     Maintains Fast Buffer & Provides additional info in Log like Process id, thread id datetime etc
    ///     Also implemented IControlledShutDown to handler Stopping of Logging with or without completeing pending task
    /// </summary>
    public abstract class AbstractLoggerAdapter : IEventLogger, IControlledShutDown,IDisposable
    {
        private bool _disposed;

        /// <summary>
        ///     The threadsafe fast buffer to which the logging will be done.
        ///     Scalable datastructure for multiple concurrent producer & consumers
        ///     Advantage of BlockingCollection is Consumer need not to implement th eidle scenarios when queue is empty
        /// </summary>
        protected readonly BlockingCollection<LogData> FastBuffer;

        /// <summary>
        /// to cancel the task that is consuming the collection
        /// </summary>
        private readonly CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        ///     The abstract Adaptor that will log the
        /// </summary>
        /// <param name="bufferCollection">
        ///     Any data structure implementing IProducerConsumerCollection Eg. ConcurrentQueue or
        ///     ConcurrentDictionary or ConcuurrentBag or CustomCollcetion
        /// </param>
        protected AbstractLoggerAdapter(IProducerConsumerCollection<LogData> bufferCollection)
        {
            if (bufferCollection == null)
            {
                return;
            }
            FastBuffer = new BlockingCollection<LogData>(bufferCollection);
            //can set additional properties here like max buffer size, etc

            //start a consumer on independent new thread to avoid consumer blocking the calling thread
            _cancellationTokenSource = new CancellationTokenSource();
            Task.Factory.StartNew(StartConsumer, _cancellationTokenSource.Token);
        }
            
        /// <summary>
        ///     The actual Consumer method that will consume the collection
        /// </summary>
        /// <param name="data">The data to be logged </param>
        protected abstract void WriteLog(LogData data);

        /// <summary>
        ///     Entry Point of Logging the message. Must return as soon as possible without much processing here.
        ///     hence queuing to Fast buffer & returinng
        /// </summary>
        public void LogMessage(string message, Severity severity)
        {
            try
            {
                FastBuffer?.TryAdd(new LogData
                {
                    TimeStamp = DateTime.Now,
                    Process = Process.GetCurrentProcess().Id,
                    Thread = Thread.CurrentThread.ManagedThreadId,
                    Message = message,
                    Severity = severity
                });
            }
            catch (Exception e)
            {
                //this case will never arrive unless someone is trying to write the log after its disposed
            }
        }

        private void StartConsumer()
        {
            try
            {
                while (true)
                {
                    _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                    var data = FastBuffer.Take();

                 

                    //this tryTake being blocking collection will wait as long as new items are not available & resume when some new items are queued
                    if (data!=null)
                    {
                       
                        WriteLog(data);
                    }
                }
            }
            catch (InvalidOperationException e)
            {
                // we are done logging means no more producers will queue new entries.
            }
            catch (Exception e)
            {
                //this case will never come but in case derived classes dont handle it we have a backshield
            }
        }

        public void ShutDown(bool forceShutDown = false)
        {
            //to notify consumer no longer new items will be added to the collection
            FastBuffer.CompleteAdding();
           
            if (forceShutDown)
            {
                _cancellationTokenSource.Cancel();
                FastBuffer.Dispose();
            }
        }

        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Implementation of dispose pattern
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                ShutDown();
            }

            _disposed = true;
        }

        ~AbstractLoggerAdapter()
        {
            Dispose(false);
        }

        
    }
}