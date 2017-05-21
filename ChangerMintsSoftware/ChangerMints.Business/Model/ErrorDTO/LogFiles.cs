using System;
using log4net;

namespace ChangerMints.Business {
    public enum LogType {
        Info,
        Error
    }

    public class LogFiles {
        private static readonly ILog log = LogManager.GetLogger(
           System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void LogError(string message, Exception ex, LogType type) {
            log4net.Config.XmlConfigurator.Configure();
            if (LogType.Error == type)
                log.Error(message, ex);

            if (LogType.Info == type)
                log.Info(message);
        }
    }
}