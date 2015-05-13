using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace frauddetect.common.core.logging
{
    sealed class Logger
    {
        private bool IsInitialized;
        private Object Lock = new Object();

        public void Initialize(string applicationName, string configFilePath)
        {
            if(string.IsNullOrWhiteSpace(applicationName))
            {
                throw new Exception("Application name is empty.");
            }

            if(string.IsNullOrWhiteSpace(configFilePath))
            {
                throw new Exception("Configuration file path is empty.");
            }

            if(!File.Exists(configFilePath))
            {
                throw new Exception("Configuration file doesn't exist.");
            }

            if (IsInitialized)
            {
                return;
            }

            lock (Lock)
            {
                XmlConfigurator.Configure();
                GlobalContext.Properties["ApplicationName"] = applicationName;

                XmlConfigurator.ConfigureAndWatch(new FileInfo(configFilePath));

                IsInitialized = true;
            }
        }

        public ILog GetLogger(Type T)
        {
            if (IsInitialized == false)
            {
                throw new Exception("Logger isn't initialized.");
            }

            return LogManager.GetLogger(T);
        }
    }
}
