using System;
using System.Threading.Tasks;
using LoggerComponent;

namespace ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            //Initialize logger 
            var logger = Logger.GetLogger();

            for (var i = 50; i >= 0; i--)
            {
                var i1 = i;
                Task.Factory.StartNew(() => Logger.LogInfo($"{i1}"));
                Task.Factory.StartNew(() => Logger.LogWarning($"{i1}"));
                Task.Factory.StartNew(() => Logger.LogError($"{i1}"));
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        throw new InvalidOperationException("Some Custom Invalid Operation Message");
                    }
                    catch (Exception e)
                    {
                        Logger.LogError(e);
                    }
                });

                if (i == 25)
                {
                    //stop after finishing pending task
                    Task.Factory.StartNew(() =>
                    {
                        logger.ShutDown();
                    });

                    //stop with force abort
                    //Task.Factory.StartNew(() => logger.ShutDown(true));

                    //break;
                }
            }
            Console.ReadLine();
        }
    }
}
