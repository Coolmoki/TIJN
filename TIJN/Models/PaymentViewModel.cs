using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TIJN.Models
{
    public class PaymentViewModel
    {
        public IEnumerable<payment> RequestedPayments { get; set; }
        public IEnumerable<payment> SentPayments { get; set; }
    }
}