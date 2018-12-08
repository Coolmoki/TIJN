using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TIJN.Models
{
    public class UserViewModel
    {
        public int userID { get; set; }
        public int planID { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public Nullable<int> SSN { get; set; }
        public decimal balance { get; set; }
        public string email { get; set; }
        public Nullable<int> phoneNumber { get; set; }
        public string password { get; set; }
        public Nullable<int> loginStatus { get; set; }
        public BankAccount bankAccount { get; set; }
    }
}