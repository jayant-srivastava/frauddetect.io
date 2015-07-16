using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using frauddetect.common.core.web;
using frauddetect.api.transaction.service;

namespace frauddetect.unittests.api
{
    [TestClass]
    public class transactionservicetests
    {
        [TestMethod]
        public void POSTMethod_Valid()
        {
            TransactionOutput output = null;
            bool success = false;

            try
            {
                TransactionInput input = new TransactionInput()
                {
                    AccountNumber = "XXXX-XXXXX",
                    Amount = 2.0,
                    Store = "Macys"
                };

                output = new WebManager().POSTMethod<TransactionInput, TransactionOutput>(@"http://localhost/services/v1/transaction.service/Transaction.svc/authorize", input);
                success = true;
            }
            catch (Exception)
            {
            }

            Assert.IsNotNull(output);
            Assert.IsTrue(success == true);
        }
    }
}
