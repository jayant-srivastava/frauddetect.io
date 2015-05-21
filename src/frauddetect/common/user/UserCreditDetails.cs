using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace frauddetect.common.user
{
    [Serializable]
    public class UserCreditDetails
    {
        public string UserId { get; set; }
        public string Account { get; set; }
        public int ExpiryYear { get; set; }
        public int ExpiryMonth { get; set; }
        public int CVV { get; set; }
        public CreditType CreditType { get; set; }
    }
}
