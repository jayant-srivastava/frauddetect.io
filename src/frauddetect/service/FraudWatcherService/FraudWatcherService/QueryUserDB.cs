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
        public string GetUserAccountFromLog(string fullFilePath, ref string transactionAmount)
        {
            try
            {
                string csvPath = ConvertGZ2CSV(fullFilePath);

                //Search for Account Number
                string userAccountNumber = ExtractSearchResultFromCSV(csvPath, ref transactionAmount); ;

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
            watcher.Filter = "*.csv.gz";
            watcher.IncludeSubdirectories = true;
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.EnableRaisingEvents = true;
            log.Debug("Exiting WatchForNewRecords");
        }

        public void OnChanged(object sender, FileSystemEventArgs inArgs)
        {
            try
            {
                log.Debug("New record found");
                //Read the file contents
                string fileName = inArgs.Name;
                string fullFilePath = inArgs.FullPath;
                log.Debug("File name and path : " + fileName + " - " + fullFilePath);
                try
                {
                    FileInfo info = new FileInfo(fullFilePath);
                    
                    if (info.Name.EndsWith("results.sra.csv.gz"))
                    {
                        log.Debug("File extension : " + info.Extension + " and file Name : " + info.Name + " - Records ignored");
                        return;
                    }
                    else
                    {
                        log.Debug("File extension : " + info.Extension + " and file Name : " + info.Name);
                    }
                }
                catch (Exception e)
                {
                    log.Debug(e.StackTrace);
                }
                string transactionAmount = string.Empty;
                string accountNumber = GetUserAccountFromLog(fullFilePath, ref transactionAmount);
                log.Debug(fileName + " - " + fullFilePath + " - " + accountNumber);

                if (accountNumber == string.Empty)
                    return;

                //Lookup MongoDB User database for the account number
           
                UserCreditDetailManager ucdManager = new UserCreditDetailManager();
                ucdManager.Initialize(ConfigurationManager.AppSettings["MongoDB_URL"]);
                UserCreditDetail ucd = ucdManager.GetByAccount(accountNumber);

                UserManager userManager = new UserManager();
                userManager.Initialize(ConfigurationManager.AppSettings["MongoDB_URL"]);
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
                    //Send email to the user
                    EmailManager mgr = new EmailManager("xxxxxxx@gmail.com", user.EmailAddress, "Potential Credit Card Fraud", "Please check your credit card records urgently : Credit transaction of USD : " + transactionAmount);
                    mgr.MailServer = "smtp.gmail.com";
                    mgr.SmtpPort = 587;
                    mgr.EnableEmailSSL = true;
                    mgr.EmailAccountCredential = new System.Net.NetworkCredential("xxxxxxxx@gmail.com", "xxxxxxx");
                    mgr.Send();
                    log.Debug("Message sent to user : " + user.EmailAddress);

                    //Send also a text message to the user
                    if (user.Phone != string.Empty) 
                    {
                        EmailManager txtManager = null;
                        if (transactionAmount != string.Empty)
                        {
                            txtManager = new EmailManager("xxxxxxx@gmail.com", user.Phone, "Potential Credit Card Fraud", "Please check your credit card records urgently : Credit transaction of USD : " + transactionAmount);
                        }
                        else
                        {
                            txtManager = new EmailManager("xxxxxxxx@gmail.com", user.Phone, "Potential Credit Card Fraud", "Please check your credit card records urgently");
                        }
                        txtManager.MailServer = "smtp.gmail.com";
                        txtManager.SmtpPort = 587;
                        txtManager.EnableEmailSSL = true;
                        txtManager.EmailAccountCredential = new System.Net.NetworkCredential("xxxxxxx@gmail.com", "xxxxxxx");
                        txtManager.Send();
                        log.Debug("Message sent to user : " + user.Phone);

                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
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

        public string ExtractSearchResultFromCSV(string csvFileName, ref string transactionAmount)
        {
            try
            {
                int counter = 0;
                int cellValues = 0;
                var reader = new StreamReader(File.OpenRead(csvFileName));
                Dictionary<string, string> dictionaryA = new Dictionary<string, string>();

                while (!reader.EndOfStream)
                {
                    var line1 = reader.ReadLine();
                    counter++;
                    var values1 = line1.Split(',');

                    var line2 = reader.ReadLine();
                    counter++;
                    var values2 = line2.Split(',');
                  
                    
                    foreach (var val in values1)
                    {
                        dictionaryA[val] = values2[cellValues];
                        cellValues++;
                    }

                    if (counter > 2)
                        break;
                }
                reader.Close();

                string searchTerm, totalAmount;

                if (dictionaryA.ContainsKey("AccountNumber"))
                    searchTerm = dictionaryA["AccountNumber"];
                else
                    searchTerm = string.Empty;

                if (dictionaryA.ContainsKey("TotalAmount"))
                    totalAmount = dictionaryA["TotalAmount"];
                else
                    totalAmount = string.Empty;

                if (totalAmount == string.Empty)
                {
                    if (dictionaryA.ContainsKey("Amount"))
                        totalAmount = dictionaryA["Amount"];
                    else
                        totalAmount = string.Empty;
                }

                log.Debug("TotalAmount : " + totalAmount);

                transactionAmount = totalAmount;

                return searchTerm;
            }
            catch (Exception ex)
            {
                log.Debug("Error in ExtractSearchResultFromCSV : " + ex.StackTrace);
                return string.Empty;
            }
        }
    }
}
