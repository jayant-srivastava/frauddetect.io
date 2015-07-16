using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace frauddetect.common.user
{
    [Serializable]
    public class UserCreditDetail
    {
        [BsonId]
        public ObjectId ID { get; set; }
        public string PrimaryUserId { get; set; }
        public string SecondaryUserId { get; set; }
        public string Account { get; set; }
        public int ExpiryYear { get; set; }
        public int ExpiryMonth { get; set; }
        public int CVV { get; set; }
        public CreditCardType CreditType { get; set; }
        public bool Active { get; set; }
        public double Limit { get; set; }
        public double Balance { get; set; }
    }
}
