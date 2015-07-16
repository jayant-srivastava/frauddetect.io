using Splunk;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;

namespace frauddetect.api.fraud.service
{
    public class Fraud : IFraud
    {
        public List<FraudOutput> FraudDetails(int months = 0)
        {
            Service splunkService = null;

            try
            {
                #region Connect to Splunk Server

                ServiceArgs serviceArgs = new ServiceArgs()
                {
                    Host = ConfigurationManager.AppSettings["SPLUNKSERVER"],
                    Port = int.Parse(ConfigurationManager.AppSettings["SPLUNKPORT"]),
                };

                splunkService = new Service(serviceArgs);
                splunkService.Login(ConfigurationManager.AppSettings["SPLUNKUSER"], ConfigurationManager.AppSettings["SPLUNKPASSWORD"]);

                #endregion

                #region Create Job parameters

                DateTime LatestTime = DateTime.UtcNow;
                DateTime EarliestTime = (months > 0) ? LatestTime.AddMonths(-months) : LatestTime.AddDays(-1);

                JobArgs jobArgs = new JobArgs()
                {
                    EarliestTime = EarliestTime.ToString("yyyy-MM-ddT12:00:00.000-07:00"),
                    LatestTime = LatestTime.ToString("yyyy-MM-ddT12:00:00.000-07:00"),
                };

                jobArgs.ExecutionMode = JobArgs.ExecutionModeEnum.Normal;
                string query = "search * | head 50000 | search source=\"C:\\\\frauddetect.io\\\\support\\\\logs\\\\fraud.server.log\" | fields amount,latitude,longitude";

                #endregion

                #region Create Splunk Search Job and wait for it to finish

                Job job = splunkService.GetJobs().Create(query, jobArgs);
                while (!job.IsDone)
                {
                    try
                    {
                        Thread.Sleep(500);
                    }
                    catch (ThreadInterruptedException e)
                    {
                        Trace.WriteLine(e.Message);
                        throw new Exception("Job failed.", e.InnerException);
                    }
                }

                #endregion

                #region Parse the results returned by Splunk

                var outArgs = new JobResultsArgs
                {
                    OutputMode = JobResultsArgs.OutputModeEnum.Xml,
                    Count = 0,
                };

                List<FraudOutput> output = new List<FraudOutput>();
                using (var stream = job.Results(outArgs))
                {
                    using (var rr = new ResultsReaderXml(stream))
                    {
                        foreach (var @event in rr)
                        {
                            FraudOutput fraudOutput = new FraudOutput();

                            foreach (string key in @event.Keys)
                            {
                                switch (key)
                                {
                                    case "amount":
                                        fraudOutput.Amount = double.Parse(@event[key]);
                                        break;
                                    case "longitude":
                                        fraudOutput.Longitude = double.Parse(@event[key]);
                                        break;
                                    case "latitude":
                                        fraudOutput.Latitude = double.Parse(@event[key]);
                                        break;
                                }
                            }

                            output.Add(fraudOutput);
                        }
                    }
                }

                #endregion

                return output;
            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex.Message);
                return new List<FraudOutput>();
            }
            finally
            {
                if(splunkService != null)
                {
                    splunkService.Logout();
                }
            }
        }
    }
}
