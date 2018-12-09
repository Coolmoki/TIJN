using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TIJN;

namespace TIJN.Controllers
{
    public class BankAccountsController : Controller
    {
        private TIJNEntities db = new TIJNEntities();

        // GET: BankAccounts
        public ActionResult Index(int? userId)
        {
            ViewBag.currentUserId = userId;
            var bankAccounts = db.BankAccounts.Where(b => b.userID == userId).Include(b => b.User);
            return View(bankAccounts.ToList());
        }

        // GET: BankAccounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccount bankAccount = db.BankAccounts.Find(id);
            if (bankAccount == null)
            {
                return HttpNotFound();
            }
            return View(bankAccount);
        }

        // GET: BankAccounts/Create
        public ActionResult Create(int? userId)
        {
            ViewBag.userID = new SelectList(db.Users.Where(b => b.userID == userId), "userID", "firstName");
            return View();
        }

        // GET: BankAccounts/Deposit
        public ActionResult Deposit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccount bankAccount = db.BankAccounts.Find(id);
            TIJN.Models.TransferViewModel transferVM = new TIJN.Models.TransferViewModel();
            transferVM.balance = bankAccount.User.balance.Value;
            transferVM.bankaccountID = bankAccount.bankaccountID;
            transferVM.userID = bankAccount.User.userID;
            if (bankAccount == null)
            {
                return HttpNotFound();
            }
            return View(transferVM);
        }

        // GET: BankAccounts/Withdraw
        public ActionResult Withdraw(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccount bankAccount = db.BankAccounts.Find(id);
            TIJN.Models.TransferViewModel transferVM = new TIJN.Models.TransferViewModel();
            transferVM.balance = bankAccount.balance;
            transferVM.bankaccountID = bankAccount.bankaccountID;
            transferVM.userID = bankAccount.User.userID;
            if (bankAccount == null)
            {
                return HttpNotFound();
            }
            return View(transferVM);
        }

        // POST: BankAccounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "bankaccountID,accountNumber,userID,isPrimary,isVerified,balance")] BankAccount bankAccount)
        {
            if (ModelState.IsValid)
            {
                db.BankAccounts.Add(bankAccount);
                db.SaveChanges();
                return RedirectToAction("Index", new { userId = bankAccount.userID });
            }

            ViewBag.userID = new SelectList(db.Users, "userID", "firstName", bankAccount.userID);
            return View(bankAccount);
        }

        // GET: BankAccounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccount bankAccount = db.BankAccounts.Find(id);
            if (bankAccount == null)
            {
                return HttpNotFound();
            }
            ViewBag.userID = new SelectList(db.Users, "userID", "firstName", bankAccount.userID);
            return View(bankAccount);
        }

        // POST: BankAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "bankaccountID,accountNumber,userID,isPrimary,isVerified,balance")] BankAccount bankAccount)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bankAccount).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { userId = bankAccount.userID });
            }
            ViewBag.userID = new SelectList(db.Users, "userID", "firstName", bankAccount.userID);
            return View(bankAccount);
        }

        // GET: BankAccounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccount bankAccount = db.BankAccounts.Find(id);
            if (bankAccount == null)
            {
                return HttpNotFound();
            }
            return View(bankAccount);
        }

        // POST: BankAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BankAccount bankAccount = db.BankAccounts.Find(id);
            db.BankAccounts.Remove(bankAccount);
            db.SaveChanges();
            return RedirectToAction("Index", new { userId = bankAccount.userID });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // POST: BankAccounts/Withdraw
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Withdraw(TIJN.Models.TransferViewModel transferVM)
        {
            BankAccount bankAccount = db.BankAccounts.Find(transferVM.bankaccountID);
            User user = db.Users.Find(transferVM.userID);
            if (ModelState.IsValid)
            {
                db.Entry(bankAccount).State = EntityState.Modified;
                bankAccount.balance = bankAccount.balance - transferVM.amount;
                db.SaveChanges();
                db.Entry(user).State = EntityState.Modified;
                user.balance = user.balance + transferVM.amount;
                db.SaveChanges();
                return RedirectToAction("Index", new { userId = bankAccount.userID });
            }

            return View(bankAccount);
        }

        // POST: BankAccounts/Deposit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Deposit(TIJN.Models.TransferViewModel transferVM)
        {
            BankAccount bankAccount = db.BankAccounts.Find(transferVM.bankaccountID);
            User user = db.Users.Find(transferVM.userID);
            if (ModelState.IsValid)
            {
                db.Entry(bankAccount).State = EntityState.Modified;
                bankAccount.balance = bankAccount.balance + transferVM.amount;
                db.SaveChanges();
                db.Entry(user).State = EntityState.Modified;
                user.balance = user.balance - transferVM.amount;
                db.SaveChanges();
                return RedirectToAction("Index", new { userId = bankAccount.userID });
            }

            return View(bankAccount);
        }
    }
}
