using System;
using System.Threading;
 
 
namespace ChainOfResponsibility
{
    [Flags]
    public enum LogLevel
    {
        None = 0,                 //        0
        Info = 1,                 //        1
        Debug = 2,                //       10
        Warning = 4,              //      100
        Error = 8,                //     1000
        FunctionalMessage = 16,   //    10000
        FunctionalError = 32,     //   100000
        All = 63                  //   111111
    }
 
    /// <summary>
    /// Abstract Handler in chain of responsibility pattern.
    /// </summary>
    public abstract class Logger
    {
        protected LogLevel logMask;
 
        // The next Handler in the chain
        protected Logger next;
 
        public Logger(LogLevel mask)
        {
            this.logMask = mask;
        }
 
        /// <summary>
        /// Sets the Next logger to make a list/chain of Handlers.
        /// </summary>
        public Logger SetNext(Logger nextlogger)
        {
            Logger lastLogger = this;

            while(lastLogger.next != null)
            {
                lastLogger = lastLogger.next;
            }

            lastLogger.next = nextlogger;
            return this;
        }
 
        public void Message(string msg, LogLevel severity)
        {
            if ((severity & logMask) != 0) //True only if any of the logMask bits are set in severity
            {
                WriteMessage(msg);
            }
            if (next != null) 
            {
                next.Message(msg, severity); 
            }
        }
 
        abstract protected void WriteMessage(string msg);
    }
 
    public class ConsoleLogger : Logger
    {
        public ConsoleLogger(LogLevel mask)
            : base(mask)
        { }
 
        protected override void WriteMessage(string msg)
        {
            Console.WriteLine("Writing to console: " + msg);
        }
    }
 
    public class EmailLogger : Logger
    {
        public EmailLogger(LogLevel mask)
            : base(mask)
        { }
 
        protected override void WriteMessage(string msg)
        {
            // Placeholder for mail send logic, usually the email configurations are saved in config file.
            Console.WriteLine("Sending via email: " + msg);
        }
    }
 
    class FileLogger : Logger
    {
        public FileLogger(LogLevel mask)
            : base(mask)
        { }
 
        protected override void WriteMessage(string msg)
        {
            // Placeholder for File writing logic
            Console.WriteLine("Writing to Log File: " + msg);
        }
    }
 
    public class Program
    {
        public static void Main(string[] args)
        {
            // Build the chain of responsibility
            Logger logger;
            logger = new ConsoleLogger(LogLevel.All)
                             .SetNext(new EmailLogger(LogLevel.FunctionalMessage | LogLevel.FunctionalError))
                             .SetNext(new FileLogger(LogLevel.Warning | LogLevel.Error));
 
            // Handled by ConsoleLogger since the console has a loglevel of all
            logger.Message("Entering function ProcessOrder().", LogLevel.Debug);
            logger.Message("Order record retrieved.", LogLevel.Info);
 
            // Handled by ConsoleLogger and FileLogger since filelogger implements Warning & Error
            logger.Message("Customer Address details missing in Branch DataBase.", LogLevel.Warning);
            logger.Message("Customer Address details missing in Organization DataBase.", LogLevel.Error);
 
            // Handled by ConsoleLogger and EmailLogger as it implements functional error
            logger.Message("Unable to Process Order ORD1 Dated D1 For Customer C1.", LogLevel.FunctionalError);
 
            // Handled by ConsoleLogger and EmailLogger
            logger.Message("Order Dispatched.", LogLevel.FunctionalMessage);
        }
    }
}
