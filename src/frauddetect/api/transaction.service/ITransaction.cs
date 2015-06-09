using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace transaction.service
{
    [ServiceContract]
    public interface ITransaction
    {
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/authorize", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        TransactionOutput Authorize(TransactionInput transactionInput);

        [OperationContract]
        [WebGet(UriTemplate = "/health", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        bool Health();
    }

    [DataContract]
    [Serializable]
    public class TransactionInput
    {
        [DataMember]
        public string AccountNumber { get; set; }

        [DataMember]
        public string AccountName { get; set; }

        [DataMember]
        public int ExpiryMonth { get; set; }

        [DataMember]
        public int ExpiryYear { get; set; }

        [DataMember]
        public int CVV { get; set; }

        [DataMember]
        public string Store { get; set; }

        [DataMember]
        public double Amount { get; set; }
    }

    [DataContract]
    [Serializable]
    public class TransactionOutput
    {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public StatusCode StatusCode { get; set; }

        [DataMember]
        public string AuthorizationCode { get; set; }
    }

    [DataContract]
    [Serializable]
    public enum StatusCode
    {
        Verifying = -1,
        Failed = 100,

        InvalidUser = 200,
        InvalidUserIsInActive = 201,

        InvalidAccountNumber = 301,
        InvalidAccountName = 302,
        InvalidAmount = 303,
        InvalidExpiryYear = 304,
        InvalidExpiryMonth = 305,
        InvalidCVV = 306,
        InvalidAccountInActive = 307,

        Success = 0,
    }
}
