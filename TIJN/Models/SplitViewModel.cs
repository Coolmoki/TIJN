using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TIJN.Models
{
    public class SplitViewModel
    {
        public int paymentID { get; set; }
        public List<int> payorUserID { get; set; }
        public decimal amount { get; set; }
        public string memo { get; set; }
        public int payeeUserID { get; set; }
        public int status { get; set; }

        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}