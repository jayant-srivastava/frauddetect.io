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
    public class User
    {
        [BsonId]
        public ObjectId ID { get; set; }

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        public string SSN { get; set; }

        public string EmailAddress { get; set; }
        public string Phone { get; set; }

        public string BillingHouseNumber { get; set; }
        public string BillingStreetAddress { get; set; }
        public string BillingCity { get; set; }
        public string BillingZipCode { get; set; }
        public string BillingCountry { get; set; }

        public string DOB { get; set; }

        public bool Active { get; set; }
    }
}
