using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace frauddetect.api.transaction.service
{
    sealed class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostCode { get; set; }
        public string Country { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public override string ToString()
        {
            return string.Format("Street: {0}, City: {1}, State: {2}, PostCode: {3}, Country: {4}, Latitude: {5}, Longitude: {6}",
                Street, City, State, PostCode, Country, Latitude, Longitude);
        }
    }
}