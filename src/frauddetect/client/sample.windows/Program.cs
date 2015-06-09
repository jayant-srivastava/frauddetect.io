﻿using frauddetect.common.core.logging;
using frauddetect.common.core.web;
using frauddetect.common.user;
using frauddetect.common.user.manager;
using log4net;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using transaction.service;

namespace sample.windows
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                int random = new Random().Next(1000000000);

                #region variables

                string firstName = "John";
                string lastName = string.Format("Doe {0}", random);
                string SSN = random.ToString();
                string accountNumber = string.Format("1111111{0}", random);
                string email = string.Format("{0}.gmail.com", random);
                string phone = random.ToString();

                #endregion

                #region Create User

                Console.WriteLine("Creating user...");

                User user1 = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Active = true,
                    SSN = SSN,
                    EmailAddress = email,
                    Phone = phone,
                };

                UserManager userManager = new UserManager();
                userManager.Initialize(ConfigurationManager.AppSettings["mongo"]);

                ObjectId id = userManager.Insert(user1);
                Console.WriteLine("Created user with Id: " + id);

                #endregion

                #region Create credit card detail

                Console.WriteLine("Creating credit detail...");

                UserCreditDetailManager userCreditDetailManager = new UserCreditDetailManager();
                userCreditDetailManager.Initialize(ConfigurationManager.AppSettings["mongo"]);

                UserCreditDetail userCreditDetail = new UserCreditDetail()
                {
                    Account = accountNumber,
                    CVV = 1111,
                    Active = true,
                    Limit = 5000,
                    Balance = 0,
                    CreditType = CreditCardType.Visa,
                    ExpiryYear = 2016,
                    ExpiryMonth = 12,
                    PrimaryUserId = id.ToString(), 
                };

                userCreditDetailManager.Insert(userCreditDetail);

                Console.WriteLine("Created credit detail with account #: " + accountNumber);

                #endregion

                #region Simulate transaction 

                Console.WriteLine("Simulating transaction...");

                TransactionInput input = new TransactionInput()
                {
                    AccountName = firstName + " " + lastName,
                    AccountNumber = accountNumber,
                    Amount = 2.0,
                    Store = "Macys",
                    CVV = 1111,
                    ExpiryMonth = 12,
                    ExpiryYear = 2016,
                };

                TransactionOutput output = new WebManager().POSTMethod<TransactionInput, TransactionOutput>(string.Format(@"http://{0}/services/v1/transaction.service/Transaction.svc/authorize", ConfigurationManager.AppSettings["webservice"]), input);
                if(output != null && output.Success == true)
                {
                    Console.WriteLine("Transaction successfull.");
                }
                else
                {
                    Console.WriteLine("Transaction failed.");
                }

                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
