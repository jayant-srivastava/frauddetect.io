using System;
//using MongoDB.Driver;
using MongoDB.Bson;
using log4net;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using frauddetect.common.core.email;
using frauddetect.common.core.logging;
using frauddetect.common.user;
using frauddetect.common.user.manager;
using System.Collections.Generic;
using System.Diagnostics;

namespace frauddetect.service.fraudwatcher
{
    public class QueryUserDB
    {
        common.core.logging.LogManager logger = new common.core.logging.LogManager();
        ILog log;

        public QueryUserDB()
        {
            //Debugger.Launch();
            logger.Initialize("CheckRecords", @"C:\FWService\logging.config");
            log = logger.GetLogger(typeof(QueryUserDB));
            WatchForNewRecords();
        }

        /// <summary>
        ///     Data from Splunk log
        /// </summary>
        public string GetUserAccountFromLog(string fullFilePath)
        {
            try
            {
                string csvPath = ConvertGZ2CSV(fullFilePath);
                string searchTerm = ExtractSearchResultFromCSV(csvPath);

                //Search for Account Number
                string userAccountNumber = string.Empty;
                List<string> contents = new List<string>(searchTerm.Split(' '));
                log.Debug("Find the account number : " + contents);
                userAccountNumber = contents.Find(s => (s.Length == 16) && (s.StartsWith("1111")));

                return userAccountNumber;
            }
            catch (Exception ex)
            {
                log.Debug("Error in GetUserAccountFromLog : " + ex.StackTrace);
                return string.Empty;
            }
        }

        private void WatchForNewRecords()
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = ConfigurationManager.AppSettings["SplunkRecordFolderPath"];
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Filter = "*.*";
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.EnableRaisingEvents = true;
        }

        public void OnChanged(object sender, FileSystemEventArgs inArgs)
        {
            try
            {
                log.Debug("New record found");
                //Read the file contents
                string fileName = inArgs.Name;
                string fullFilePath = inArgs.FullPath;
                string accountNumber = GetUserAccountFromLog(fullFilePath);
                log.Debug(fileName + " - " + fullFilePath + " - " + accountNumber);

                if (accountNumber == string.Empty)
                    return;

                //Lookup MongoDB User database for the account number
                UserCreditDetailManager ucdManager = new UserCreditDetailManager();
                UserCreditDetail ucd = ucdManager.GetByAccount(accountNumber);

                UserManager userManager = new UserManager();
                ObjectId userId;
                User user = null;
                if (ObjectId.TryParse(ucd.PrimaryUserId, out userId))
                {
                    user = userManager.GetById(userId);
                }
                log.Debug("User details : " + user.FirstName + " " + user.LastName);

                //Send email/text to the User
                if (user != null)
                {
                    EmailManager mgr = new EmailManager("jsy.ventures@gmail.com", user.EmailAddress, "Potential security issue", "Please check your credit card");
                    mgr.Send();
                    log.Debug("Message sent to user : " + user.EmailAddress);
                }
            }
            catch (Exception ex)
            {
                log.Debug("Error in OnChanged : " + ex.StackTrace);
            }
        }

        public string ConvertGZ2CSV(string filePath)
        {
            try
            {
                Guid guid = Guid.NewGuid();
                string fileName = @"C:\SplunkRecords\test_" + guid + ".csv";

                using (Stream fd = File.Create(fileName))
                using (Stream fs = File.OpenRead(filePath))
                using (Stream csStream = new GZipStream(fs, CompressionMode.Decompress))
                {
                    byte[] buffer = new byte[10240];
                    int nRead;
                    while ((nRead = csStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fd.Write(buffer, 0, nRead);
                    }
                }
                log.Debug("File extracted to : " + fileName);
                return fileName;
            }
            catch (Exception ex)
            {
                log.Debug("Error in ConvertGZtoCSV : " + ex.StackTrace);
                return string.Empty;
            }
        }

        public string ExtractSearchResultFromCSV(string csvFileName)
        {
            try
            {
                var reader = new StreamReader(File.OpenRead(csvFileName));
                List<string> listA = new List<string>();
                List<string> listB = new List<string>();
                while (!reader.EndOfStream)
                {
                    var line1 = reader.ReadLine();
                    var values1 = line1.Split(',');

                    listA.AddRange(values1);

                    var line2 = reader.ReadLine();
                    var values2 = line2.Split(',');

                    listB.AddRange(values2);
                }
                reader.Close();

                log.Debug("Search result : " + listB[2].ToString());
                return listB[2];
            }
            catch (Exception ex)
            {
                log.Debug("Error in ExtractSearchResultFromCSV : " + ex.StackTrace);
                return string.Empty;
            }
        }
    }
}
