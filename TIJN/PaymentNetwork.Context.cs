﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TIJN
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class TIJNEntities : DbContext
    {
        public TIJNEntities()
            : base("name=TIJNEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<RequestPayment> RequestPayments { get; set; }
        public DbSet<RequestPaymentToken> RequestPaymentTokens { get; set; }
        public DbSet<SendPayment> SendPayments { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
