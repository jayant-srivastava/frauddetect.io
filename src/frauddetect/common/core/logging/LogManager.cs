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
    public sealed class LogManager
    {
        private bool IsInitialized;
        private Object Lock = new Object();

        public void Initialize(string applicationName, string configFilePath)
        {
            if(string.IsNullOrWhiteSpace(applicationName))
            {
                throw new ArgumentException("Application name is empty.");
            }

            if(string.IsNullOrWhiteSpace(configFilePath))
            {
                throw new ArgumentException("Configuration file path is empty.");
            }

            if(!File.Exists(configFilePath))
            {
                throw new FileNotFoundException("Configuration file doesn't exist.");
            }

            if (IsInitialized)
            {
                return;
            }

            lock (Lock)
            {
                if(IsInitialized)
                {
                    return;
                }

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

            return log4net.LogManager.GetLogger(T);
        }
    }
}
