using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using transaction.service;
using frauddetect.common.core.web;

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
                    Account = "XXXX-XXXXX",
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
