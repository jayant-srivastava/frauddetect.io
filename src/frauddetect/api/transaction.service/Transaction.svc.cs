using frauddetect.common.core.logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace transaction.service
{
    public class Transaction : ITransaction
    {
        #region Private variables

        private readonly log4net.ILog _logger;
        private const string APPLICATIONNAME = "transaction.service";
        private const string LOGCONFIG = @"C:\frauddetect.io\support\configuration\logging.config";
        private readonly bool _initialized = false;

        #endregion

        #region Constructor

        Transaction()
        {
            try
            {
                LogManager logManager = new LogManager();
                logManager.Initialize(APPLICATIONNAME, LOGCONFIG);
                _logger = logManager.GetLogger(typeof(Transaction));
                _initialized = true;
            }
            catch(Exception)
            {
            }
        }

        #endregion

        #region Public APIs

        public TransactionOutput Authorize(TransactionInput transactionInput)
        {
            if (_logger != null)
            {
                _logger.Debug("START: Authorize()");
            }

            try
            {
                if(!_initialized)
                {
                    throw new Exception("Service isn't initialized.");
                }

                if (_logger != null)
                {
                    _logger.Debug(string.Format("Account: {0}, Ammount: {1}, Store: {2}", transactionInput.Account, transactionInput.Amount, transactionInput.Store));
                }

                return new TransactionOutput() { Success = true, AuthorizationCode = "100", StatusCode = 0, Message = string.Empty };
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.Error("EXCEPTION: Authorize()", ex);
                }

                return new TransactionOutput() { Success = false, AuthorizationCode = string.Empty, StatusCode = -1, Message = ex.Message };
            }
            finally
            {
                if (_logger != null)
                {
                    _logger.Debug("END: Authorize()");
                }
            }
        }

        public bool Health()
        {
            if (_logger != null)
            {
                _logger.Debug("START: Health()");
            }

            try
            {
                if (!_initialized)
                {
                    throw new Exception("Service isn't initialized.");
                }

                return true;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.Error("EXCEPTION: Health()", ex);
                }

                return false;
            }
            finally
            {
                if (_logger != null)
                {
                    _logger.Debug("END: Health()");
                }
            }
        }

        #endregion
    }
}
