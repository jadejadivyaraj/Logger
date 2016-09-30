using LoggerComponent.Model;

namespace LoggerComponent.Adapters
{
    public class DefaultLoggingAdapter : AbstractLoggerAdapter
    {
        public DefaultLoggingAdapter() : base(null)
        {
            // No consumer for default logging adapter  hence this adapter doesnt log anything
        }

        protected override void WriteLog(LogData data)
        {
            // nothing to consume as nothing we are writting
        }
    }
}
