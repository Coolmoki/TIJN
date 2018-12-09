using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TIJN.Models
{
    public class TransferViewModel
    {
        public decimal amount { get; set; }
        public decimal balance { get; set; }
        public int bankaccountID { get; set; }
        public int userID { get; set; }
    }
}