using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using frauddetect.common.core.logging;
using System.IO;

namespace frauddetect.unittests
{
    [TestClass]
    public class commontests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void LogManager_Initialize_ApplicationName_IsNull()
        {
            new LogManager().Initialize(null, @"C:\Projects\frauddetect.io\deployment\configuration\logging.config");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void LogManager_Initialize_ApplicationName_IsEmptyString()
        {
            new LogManager().Initialize(string.Empty, @"C:\Projects\frauddetect.io\deployment\configuration\logging.config");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void LogManager_Initialize_ConfigFilePath_IsNull()
        {
            new LogManager().Initialize("Transaction.Service", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void LogManager_Initialize_ConfigFilePath_IsEmptyString()
        {
            new LogManager().Initialize("Transaction.Service", string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void LogManager_Initialize_ConfigFilePath_PathIsInvalid()
        {
            new LogManager().Initialize("Transaction.Service", @"C:\UnknowPath\logging.config");
        }

        [TestMethod]
        public void LogManager_Initialize_Valid()
        {
            bool bSuccess = false;

            try
            {
                new LogManager().Initialize("Transaction.Service", @"C:\Projects\frauddetect.io\deployment\configuration\logging.config");
                bSuccess = true;
            }
            catch(Exception)
            {
            }

            Assert.IsTrue(bSuccess == true);
        }

        [TestMethod]
        public void LogManager_GetLogger_Valid()
        {
            bool bSuccess = false;

            try
            {
                LogManager logManager = new LogManager();
                logManager.Initialize("Transaction.Service", @"C:\Projects\frauddetect.io\deployment\configuration\logging.config");

                logManager.GetLogger(typeof(commontests));

                bSuccess = true;
            }
            catch(Exception)
            {
            }

            Assert.IsTrue(bSuccess == true);
        }

        [TestMethod]
        public void LogManager_GetLogger_InValid()
        {
            bool bSuccess = false;

            try
            {
                LogManager logManager = new LogManager();
                logManager.Initialize("Transaction.Service", @"C:\Projects\frauddetect.io\deployment\configuration\logging.config");

                logManager.GetLogger(null);

                bSuccess = true;
            }
            catch (Exception)
            {
            }

            Assert.IsTrue(bSuccess == false);
        }
    }
}
