using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace frauddetect.common.user
{
    [Serializable]
    public enum CreditCardType
    {
        Visa,
        MasterCard,
        Discover,
        Amex,
    }
}
