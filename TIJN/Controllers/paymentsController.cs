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
        public ActionResult Index()
        {
            var payments = db.payments.Include(p => p.User).Include(p => p.User1);
            return View(payments.ToList());
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
        public ActionResult Create()
        {
            ViewBag.payeeUserID = new SelectList(db.Users, "userID", "firstName");
            ViewBag.payorUserID = new SelectList(db.Users, "userID", "firstName");
            return View();
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
                return RedirectToAction("Index");
            }

            ViewBag.payeeUserID = new SelectList(db.Users, "userID", "firstName", payment.payeeUserID);
            ViewBag.payorUserID = new SelectList(db.Users, "userID", "firstName", payment.payorUserID);
            return View(payment);
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
