/**************************************************************************************
' Script Name: GWLogger.cs
' **************************************************************************************
' @(#)    Purpose:
' @(#)    This is a shared component available to all Mono applications. It allows simple
' @(#)    logging to a central location. The log file name is configurable but defaults to the application name.
' @(#)    The Log is to be initialized with a specified LogVerbosity. It wraps the apache log4net component
' @(#)    writes to the log file if the LogLevel <= LogVerbosity as defined by the application
' **************************************************************************************
'  Written By: Brad Detchevery
' Created:     2019-05-29 - Initial Architecture
' 
' **************************************************************************************
'Note: Changing this routine effects all programs that log to a common
'location.
'-------------------------------------------------------------------------------*/


using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using log4net.Config;

namespace org.geekwisdom
{
        public class GWLogger
    {

        private static readonly log4net.ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
	    private static GWLogger instance = null;
        log4net.Repository.ILoggerRepository logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
        private bool isInitialized = false;
        private string LogName = "";
        private int LogVerbosity = 10;

        GWLogger()
        {
            isInitialized = false;

        }

        public static GWLogger getInstance()
        {
            if (instance != null) return instance;
            instance = new GWLogger();
            return instance;

        }

        public static GWLogger getInstance(int Verbosity)
        {
            if (instance != null) return instance;
            instance = new GWLogger();
            instance.Initialize(Verbosity);
            return instance;

        }

        private  void LoggerConstruct(int Verbosity, string LName)
        {
            LogVerbosity = Verbosity;
            LogName = LName;
        }
            
        /// <summary>
        /// Initializes the logger to use a specific config file.
        /// </summary>
        /// <param name="configFile">The path of the config file.</param>
        /// <exception cref="LoggingInitializationException">Thrown if logger is already initialized.</exception>
        /// 

        public void Initialize()
        {
            Initialize(null,10,null);
        }


    public void Initialize(int Verbosity)
    {
        Initialize(null,Verbosity,"logname.txt");
    }

    public  void Initialize(string configFile, int Verbosity = 0, String LogName = "log.txt")
        {
            if (!isInitialized)
            {
                LoggerConstruct(Verbosity, LogName);
        
                if (!String.IsNullOrEmpty(configFile))
                    XmlConfigurator.ConfigureAndWatch(logRepository, new FileInfo(configFile));
                else
                    XmlConfigurator.Configure(logRepository);
                
                isInitialized = true;
            }
            else
                throw new LoggerException("Logging has already been initialized.");
        }

        //Sub LogInfo (LogItem,LogLevel)

        public  void LogInfo(string LogItem, int LogLevel, LogType ltype = LogType.Debug, Exception exception = null)
        {
            if (LogLevel <= LogVerbosity)
            {
                if (ltype == LogType.Debug) DoLog("DebugLogger", ltype, LogItem, exception);
                else if (ltype == LogType.Security) DoLog("SecurityLogger", ltype, LogItem, exception);
                else DoLog(null, ltype, LogItem + "," + LogLevel.ToString(), exception);
            }
        }

        public void LogInfo(string LogItem, int LogLevel, LogType ltype)
        {
            LogInfo(LogItem, LogLevel, ltype, null);
        }
        public void WriteLog(int LogLevel, LogType ltype, string message, Exception exception=null)
        {

            LogInfo(message, LogLevel, ltype, exception);
        }
        private  void DoLog(string LogFile, LogType ltype, string Message, Exception exception=null)
        {
            if (LogFile == null)
            //Log to all running loggers
            {
                foreach (ILog log in GetAllLoggers())
                {
                    //Console.WriteLine("Here!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                    LogMain(log, ltype, Message, exception);
                }
            }
            else
            {
                //Log to a specific logger!
                ILog log = LogManager.GetLogger(LogFile,LogFile);
                if (log != null)
                {
                    //Console.WriteLine("Here!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!" + LogFile);
                    LogMain(log, ltype, Message, exception);
                }
                else
                    throw new LoggerException("The log \"" + LogFile + "\" does not exist or is invalid.");
            }
        }
        private  IEnumerable<ILog> GetAllLoggers()
        {
            ILog[] allLogs = LogManager.GetCurrentLoggers(logRepository.Name);
            IList<ILog> theLogs = new List<ILog>();
            for (int i = 0; i < allLogs.Length; i++)
            {
                bool isParent = false;
                for (int j = 0; j < allLogs.Length; j++)
                {
                    //Console.WriteLine(allLogs[i].Logger.Name);
                    if (i != j && allLogs[j].Logger.Name.StartsWith(string.Format("{0}.", allLogs[i].Logger.Name)))
                    {
                        isParent = true;
                        break;
                    }
                }

                if (!isParent)
                    theLogs.Add(allLogs[i]);
            }
            return theLogs;
        }
        private  void LogMain(ILog log, LogType ltype, string message, Exception exception)
        {
            if (ShouldLog(log, ltype))
            {
                switch (ltype)
                {
                    case LogType.Debug: log.Debug(message, exception); break;
                    case LogType.Info: log.Info(message, exception); break;
                    case LogType.Warning: log.Warn(message, exception); break;
                    case LogType.Error: log.Error(message, exception); break;
                    case LogType.Fatal: log.Fatal(message, exception); break;
                  //  case LogType.Security: log.Security(message, exception); break;
                }
            }

        }
        private  bool ShouldLog(ILog log, LogType ltype)
        {
            switch (ltype)
            {
                case LogType.Debug: return log.IsDebugEnabled;
                case LogType.Info: return log.IsInfoEnabled;
                case LogType.Warning: return log.IsWarnEnabled;
                case LogType.Error: return log.IsErrorEnabled;
                case LogType.Fatal: return log.IsFatalEnabled;
                case LogType.Security: return true;
                default: return false;
            }
        }
        public class LoggerException : Exception
        {
            /// <summary>
            /// Constructs a new <see cref="LoggingInitializationException"/>.
            /// </summary>
            /// <param name="message">The error message.</param>
            /// <param name="innerException">The exception which caused this exception to be thrown.</param>
            public LoggerException(string msg, Exception Exp)
                : base(msg, Exp)
            {
            }

            /// <summary>
            /// Constructs a new <see cref="LoggingInitializationException"/>.
            /// </summary>
            /// <param name="message">The error message.</param>
            public LoggerException(string msg)
                : this(msg, null)
            {
            }
        }
        public enum LogType : byte
        {
            Debug,
            Info,
            Warning,
            Error,
            Fatal,
            Security
        }
    }

}
