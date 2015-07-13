using frauddetect.common.core.logging;
using frauddetect.common.user;
using frauddetect.common.user.manager;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Xml.Linq;

namespace frauddetect.api.transaction.service
{
    public class Transaction : ITransaction
    {
        #region Private variables

        private static readonly log4net.ILog Logger;
        private const string ApplicationName = "transaction.service";
        private const string LoggerConfigFile = @"C:\frauddetect.io\support\configuration\logging.config";
        private static readonly bool Initialized = false;
        private static string MongoDb;

        #endregion

        #region Constructor

        static Transaction()
        {
            try
            {
                LogManager logManager = new LogManager();
                logManager.Initialize(ApplicationName, LoggerConfigFile);
                Logger = logManager.GetLogger(typeof(Transaction));

                MongoDb = ConfigurationManager.AppSettings["MongoDB"];
                
                Initialized = true;
            }
            catch(Exception)
            {
            }
        }

        #endregion

        #region Public APIs

        public TransactionOutput Authorize(TransactionInput transactionInput)
        {
            Guid transactionId = Guid.Empty;
            StatusCode statusCode = StatusCode.Failed;
            string accountNumber = string.Empty;

            try
            {
                IsInitialized();

                #region Validate input != null && Account# != null

                //reject request if input is null
                if (transactionInput == null) { throw new ArgumentNullException("Transaction input is null."); }

                //reject request if account number is null
                if (string.IsNullOrWhiteSpace(transactionInput.AccountNumber)) { statusCode = StatusCode.InvalidAccountNumber; throw new ArgumentException("Account number is blank."); }
                accountNumber = transactionInput.AccountNumber;

                #endregion

                //generate transaction code
                transactionId = Guid.NewGuid();

                #region Get Address details using Latitude and Longitude
                Address address = GetAddress(transactionInput.Latitude, transactionInput.Longitude);

                #endregion

                //log authorization request
                Logger.Debug(string.Format("[Transaction Id : {0}, Status Code : {1}, Account Details : [ AccountNumber : {2}, AccountName : {3} , Amount : {4} ], Store : [ StoreName : {5}, {6} ]]",
                    transactionId, Enum.GetName(typeof(StatusCode), StatusCode.Verifying), transactionInput.AccountNumber, transactionInput.AccountName, transactionInput.Amount, transactionInput.Store, address));

                #region Validate other input parameters

                //fail authorization if account name is blank
                if (string.IsNullOrWhiteSpace(transactionInput.AccountName)) { statusCode = StatusCode.InvalidAccountNumber; throw new ArgumentException("Account name is blank."); }

                //fail authorization if amount isn't > 0
                if (transactionInput.Amount <= 0) { statusCode = StatusCode.InvalidAmount; throw new ArgumentException("Amount should be greater than 0."); }

                //fail authorization if expiry year isn't valid
                if (transactionInput.ExpiryYear < DateTime.Now.Year) { statusCode = StatusCode.InvalidExpiryYear; throw new ArgumentException("Expiry year isn't valid."); }

                //fail authorization if expiry month isn't valid
                if (transactionInput.ExpiryMonth > 12 || transactionInput.ExpiryMonth < 1) { statusCode = StatusCode.InvalidExpiryMonth; throw new ArgumentException("Expiry month isn't valid."); }

                //fail authorization if CVV is empty
                if (transactionInput.CVV == 0) { statusCode = StatusCode.InvalidCVV; throw new ArgumentException("CVV isn't valid."); }

                #endregion

                #region Validate credit card is valid

                //validate credit card and amount balance
                UserCreditDetailManager userCreditDetailManager = new UserCreditDetailManager();
                userCreditDetailManager.Initialize(MongoDb);

                UserCreditDetail userCreditDetail = userCreditDetailManager.GetByAccount(transactionInput.AccountNumber);

                //fail authorization if credit details don't exist
                if (userCreditDetail == null) { statusCode = StatusCode.InvalidAccount; throw new Exception("Account is invalid."); }

                //fail authorization if account is inactive
                if (!userCreditDetail.Active) { statusCode = StatusCode.InvalidAccountInActive; throw new Exception("Account is inactive."); }

                //fail authorization if expiry year don't match
                if (userCreditDetail.ExpiryYear != transactionInput.ExpiryYear) { statusCode = StatusCode.InvalidExpiryYear; throw new Exception("Expiry year doesn't match."); }

                //fail authorization if expiry month don't match
                if (userCreditDetail.ExpiryMonth != transactionInput.ExpiryMonth) { statusCode = StatusCode.InvalidExpiryMonth; throw new Exception("Expiry month doesn't match."); }

                //fail authorization if CVV don't match
                if (userCreditDetail.CVV != transactionInput.CVV) { statusCode = StatusCode.InvalidCVV; throw new Exception("CVV doesn't match."); }

                #endregion

                #region Validate user is valid

                //validate user details
                UserManager userManager = new UserManager();
                userManager.Initialize(MongoDb);

                User primaryUser = string.IsNullOrWhiteSpace(userCreditDetail.PrimaryUserId) ? null : userManager.GetById(new ObjectId(userCreditDetail.PrimaryUserId));
                User secondaryUser = string.IsNullOrWhiteSpace(userCreditDetail.SecondaryUserId) ? null : userManager.GetById(new ObjectId(userCreditDetail.SecondaryUserId));

                //fail authorization if primary and secondary user details don't exist or match
                if (primaryUser == null && secondaryUser == null) { statusCode = StatusCode.InvalidUser; throw new Exception("User doesn't exist."); }

                User user = null;
                if (primaryUser != null && string.Compare(primaryUser.FirstName + " " + primaryUser.LastName, transactionInput.AccountName) == 0)
                {
                    user = primaryUser;
                }
                else if (secondaryUser != null && string.Compare(secondaryUser.FirstName + " " + secondaryUser.LastName, transactionInput.AccountName) == 0)
                {
                    user = secondaryUser;
                }

                if (user == null) { statusCode = StatusCode.InvalidUser; throw new Exception("User details doesn't match."); }

                //fail authorization if user is inactive
                if (!user.Active) { statusCode = StatusCode.InvalidUserIsInActive; throw new Exception("User is inactive."); }

                #endregion

                #region Check amount vs balance

                //fail authorization if balance - amount < 0
                if (userCreditDetail.Balance - transactionInput.Amount < 0) { statusCode = StatusCode.InsufficientFunds; throw new Exception("Insufficent funds."); }

                userCreditDetail.Balance = userCreditDetail.Balance - transactionInput.Amount;
                userCreditDetailManager.Update(userCreditDetail);

                #endregion

                Logger.Debug(string.Format("[Transaction Id : {0}, Status Code : {1}, Account Details : [ AccountNumber : {2}, AccountName : {3} , Amount : {4} ], Store : [ StoreName : {5}, {6} ]]",
                   transactionId, Enum.GetName(typeof(StatusCode), StatusCode.Success), transactionInput.AccountNumber, transactionInput.AccountName, transactionInput.Amount, transactionInput.Store, address));
                return new TransactionOutput() { Success = true, AuthorizationCode = transactionId.ToString(), StatusCode = StatusCode.Success, Message = string.Empty };
            }
            catch (Exception ex)
            {
                if (Logger != null) { Logger.Error(string.Format("[Transaction Id : {0}, Status Code: {1}, Account Details : [ AccountNumber: {2} ], Message: {3}]", transactionId, Enum.GetName(typeof(StatusCode), statusCode), accountNumber, ex.Message)); }
                return new TransactionOutput() { Success = false, AuthorizationCode = string.Empty, StatusCode = StatusCode.Failed, Message = "Transaction failed." };
            }
        }

        public bool Health()
        {
            try
            {
                IsInitialized();
                return true;
            }
            catch (Exception ex)
            {
                if (Logger != null) { Logger.Info("EXCEPTION: Health()", ex); }
                return false;
            }
        }

        #endregion

        #region Private functions

        private static void IsInitialized()
        {
            if (!Initialized) { throw new Exception("Service isn't initialized."); }
        }

        private Address GetAddress(double latitude, double longitude)
        {
            XDocument xdoc = XDocument.Load(string.Format(@"http://maps.googleapis.com/maps/api/geocode/xml?latlng={0},{1}&sensor=false", latitude, longitude));

            var status = (from s in xdoc.Descendants("GeocodeResponse").Descendants("status")
                          select s).FirstOrDefault();

            if (status == null || status.Value != "OK")
            {
                throw new Exception("Failed to retrieve address.");
            }

            var element = xdoc.Descendants("result").First().Descendants("address_component").Where(s => s.Descendants("type").First().Value == "route").FirstOrDefault();
            var street = string.Empty;
            if (element != null)
            {
                street = element.Descendants("long_name").First().Value;
            }

            element = xdoc.Descendants("result").First().Descendants("address_component").Where(s => s.Descendants("type").First().Value == "locality").FirstOrDefault();
            var city = string.Empty;
            if (element != null)
            {
                city = element.Descendants("long_name").First().Value;
            }

            element = xdoc.Descendants("result").First().Descendants("address_component").Where(s => s.Descendants("type").First().Value == "administrative_area_level_1").FirstOrDefault();
            var state = string.Empty;
            if (element != null)
            {
                state = element.Descendants("long_name").First().Value;
            }

            element = xdoc.Descendants("result").First().Descendants("address_component").Where(s => s.Descendants("type").First().Value == "postal_code").FirstOrDefault();
            var postcode = string.Empty;
            if (element != null)
            {
                postcode = element.Descendants("long_name").First().Value;
            }

            element = xdoc.Descendants("result").First().Descendants("address_component").Where(s => s.Descendants("type").First().Value == "country").FirstOrDefault();
            var country = string.Empty;
            if (element != null)
            {
                country = element.Descendants("long_name").First().Value;
            }
            return new Address()
            {
                Street = street,
                City = city,
                PostCode = postcode,
                State = state,
                Country = country,
                Latitude = latitude,
                Longitude = longitude,
            };
        }

        #endregion
    }
}
