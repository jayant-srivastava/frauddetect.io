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
        public string Account { get; set; }

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
        public int StatusCode { get; set; }

        [DataMember]
        public string AuthorizationCode { get; set; }
    }
}
