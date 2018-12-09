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
    public class paymentsController : Controller
    {
        private TIJNEntities db = new TIJNEntities();

        // GET: payments
        public ActionResult Index(int? userId)
        {
            ViewBag.currentUserId = userId;
            var requestedPayments = db.payments.Where(b => b.payeeUserID == userId).Include(p => p.User).Include(p => p.User1);
            var sentPayments = db.payments.Where(b => b.payorUserID == userId).Include(p => p.User).Include(p => p.User1);

            TIJN.Models.PaymentViewModel paymentVM = new TIJN.Models.PaymentViewModel();
            paymentVM.RequestedPayments = requestedPayments.ToList();
            paymentVM.SentPayments = sentPayments.ToList();
            return View(paymentVM);
        }

        // GET: payments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            payment payment = db.payments.Find(id);
            if (payment == null)
            {
                return HttpNotFound();
            }
            return View(payment);
        }

        // GET: payments/Create
        public ActionResult Create(int? userId)
        {
            ViewBag.payeeUserID = new SelectList(db.Users, "userID", "firstName");
            ViewBag.payorUserID = new SelectList(db.Users.Where(b => b.userID == userId), "userID", "firstName");
            return View();
        }

        // GET: payments/Request
        public ActionResult Request(int? userId)
        {
            ViewBag.payorUserID = new SelectList(db.Users, "userID", "firstName");
            ViewBag.payeeUserID = new SelectList(db.Users.Where(b => b.userID == userId), "userID", "firstName");
            return View();
        }

        // GET: payments/Send
        public ActionResult Send(int? userId)
        {
            ViewBag.payeeUserID = new SelectList(db.Users, "userID", "firstName");
            ViewBag.payorUserID = new SelectList(db.Users.Where(b => b.userID == userId), "userID", "firstName");
            return View();
        }

        // GET: payments/Split
        public ActionResult Split(int? userId)
        {
            ViewBag.payorUserID = new SelectList(db.Users, "userID", "firstName");
            ViewBag.payeeUserID = new SelectList(db.Users.Where(b => b.userID == userId), "userID", "firstName");
            TIJN.Models.SplitViewModel splitVM = new TIJN.Models.SplitViewModel();
            return View(splitVM);
        }

        // GET: payments/SearchSend
        public ActionResult SearchSend(int? userId)
        {
            ViewBag.payorUserID = new SelectList(db.Users.Where(b => b.userID == userId), "userID", "firstName");
            TIJN.Models.SearchSendViewModel searchSendVM = new TIJN.Models.SearchSendViewModel();
            return View(searchSendVM);
        }

        // POST: payments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "paymentID,payorUserID,amount,memo,payeeUserID,status")] payment payment)
        {
            if (ModelState.IsValid)
            {
                db.payments.Add(payment);
                db.SaveChanges();
                return RedirectToAction("Index", new { userId = payment.payorUserID });
            }

            ViewBag.payeeUserID = new SelectList(db.Users, "userID", "firstName", payment.payeeUserID);
            ViewBag.payorUserID = new SelectList(db.Users, "userID", "firstName", payment.payorUserID);
            return View(payment);
        }

        // POST: payments/Request
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Request([Bind(Include = "paymentID,payorUserID,amount,memo,payeeUserID,status")] payment payment)
        {
            User payor = db.Users.Find(payment.payorUserID);
            User payee = db.Users.Find(payment.payeeUserID);
            if (ModelState.IsValid)
            {
                db.payments.Add(payment);
                db.SaveChanges();
                db.Entry(payor).State = EntityState.Modified;
                payor.balance = payor.balance - payment.amount;
                db.SaveChanges();
                db.Entry(payee).State = EntityState.Modified;
                payee.balance = payee.balance + payment.amount;
                db.SaveChanges();
                return RedirectToAction("Index", new { userId = payment.payeeUserID });
            }

            ViewBag.payeeUserID = new SelectList(db.Users, "userID", "firstName", payment.payeeUserID);
            ViewBag.payorUserID = new SelectList(db.Users, "userID", "firstName", payment.payorUserID);
            return View(payment);
        }

        // POST: payments/Request
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Split([Bind(Include = "paymentID,payorUserID,amount,memo,payeeUserID,status")] TIJN.Models.SplitViewModel splitVM)
        {
            User payee = db.Users.Find(splitVM.payeeUserID);
            if (ModelState.IsValid)
            {
                foreach (int pUserID in splitVM.payorUserID)
                {
                    User payor = db.Users.Find(pUserID);
                    payment payment = new payment();
                    payment.memo = splitVM.memo;
                    payment.payorUserID = pUserID;
                    payment.payeeUserID = splitVM.payeeUserID;
                    payment.status = splitVM.status;
                    payment.amount = splitVM.amount;
                    db.payments.Add(payment);
                    db.SaveChanges();
                    db.Entry(payor).State = EntityState.Modified;
                    payor.balance = payor.balance - payment.amount;
                    db.SaveChanges();
                }

                db.Entry(payee).State = EntityState.Modified;
                payee.balance = payee.balance + splitVM.amount*splitVM.payorUserID.ToList().Count;
                db.SaveChanges();
                return RedirectToAction("Index", new { userId = splitVM.payeeUserID });
            }

            ViewBag.payeeUserID = new SelectList(db.Users, "userID", "firstName", splitVM.payeeUserID);
            return View(splitVM);
        }

        // POST: payments/Send
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Send([Bind(Include = "paymentID,payorUserID,amount,memo,payeeUserID,status")] payment payment)
        {
            User payor = db.Users.Find(payment.payorUserID);
            User payee = db.Users.Find(payment.payeeUserID);
            if (ModelState.IsValid)
            {
                db.payments.Add(payment);
                db.SaveChanges();
                db.Entry(payor).State = EntityState.Modified;
                payor.balance = payor.balance - payment.amount;
                db.SaveChanges();
                db.Entry(payee).State = EntityState.Modified;
                payee.balance = payee.balance + payment.amount;
                db.SaveChanges();
                return RedirectToAction("Index", new { userId = payment.payorUserID });
            }

            ViewBag.payeeUserID = new SelectList(db.Users, "userID", "firstName", payment.payeeUserID);
            ViewBag.payorUserID = new SelectList(db.Users, "userID", "firstName", payment.payorUserID);
            return View(payment);
        }

        // POST: payments/SearchSend
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchSend([Bind(Include = "paymentID,payorUserID,amount,memo,payeeUser,status")] TIJN.Models.SearchSendViewModel searchSendVM)
        {
            User payor = db.Users.Find(searchSendVM.payorUserID);
            var payee = db.Users.Where(b => b.email == searchSendVM.payeeUser || b.phoneNumber == searchSendVM.payeeUser).ToList();
            if (payee.Count>0)
            {
                var payeeUserID = payee.First().userID;
                User foundPayee = db.Users.Find(payeeUserID);
                if (ModelState.IsValid)
                {
                    payment payment = new payment();
                    payment.memo = searchSendVM.memo;
                    payment.payorUserID = searchSendVM.payorUserID;
                    payment.payeeUserID = payeeUserID;
                    payment.status = searchSendVM.status;
                    payment.amount = searchSendVM.amount;
                    db.payments.Add(payment);
                    db.SaveChanges();
                    db.Entry(payor).State = EntityState.Modified;
                    payor.balance = payor.balance - searchSendVM.amount;
                    db.SaveChanges();
                    db.Entry(foundPayee).State = EntityState.Modified;
                    foundPayee.balance = foundPayee.balance + searchSendVM.amount;
                    db.SaveChanges();
                    return RedirectToAction("Index", new { userId = searchSendVM.payorUserID });
                }
            }

            ViewBag.payorUserID = new SelectList(db.Users, "userID", "firstName", searchSendVM.payorUserID);
            return View(searchSendVM);
        }

        // GET: payments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            payment payment = db.payments.Find(id);
            if (payment == null)
            {
                return HttpNotFound();
            }
            ViewBag.payeeUserID = new SelectList(db.Users, "userID", "firstName", payment.payeeUserID);
            ViewBag.payorUserID = new SelectList(db.Users, "userID", "firstName", payment.payorUserID);
            return View(payment);
        }

        // POST: payments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "paymentID,payorUserID,amount,memo,payeeUserID,status")] payment payment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(payment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.payeeUserID = new SelectList(db.Users, "userID", "firstName", payment.payeeUserID);
            ViewBag.payorUserID = new SelectList(db.Users, "userID", "firstName", payment.payorUserID);
            return View(payment);
        }

        // GET: payments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            payment payment = db.payments.Find(id);
            if (payment == null)
            {
                return HttpNotFound();
            }
            return View(payment);
        }

        // POST: payments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            payment payment = db.payments.Find(id);
            db.payments.Remove(payment);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
