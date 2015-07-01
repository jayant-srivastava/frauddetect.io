using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace frauddetect.api.fraud.service
{
    [ServiceContract]
    public interface IFraud
    {
        [WebGet(UriTemplate = "/fraud", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<FraudOutput> FraudDetails();  
    }

    [DataContract]
    [Serializable]
    public class FraudOutput
    {
        public FraudOutput() {}

        public FraudOutput(double latitude, double longitude, double amount)
        {
            Latitude = latitude;
            Longitude = longitude;
            Amount = amount;
        }

        [DataMember]
        public double Latitude { get; set; }

        [DataMember]
        public double Longitude { get; set; }

        [DataMember]
        public double Amount { get; set; }
    }
}
