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
    public class UsersController : Controller
    {
        private TIJNEntities db = new TIJNEntities();

        // GET: Users
        public ActionResult Index()
        {
            var users = db.Users.Include(u => u.Plan);
            return View(users.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            ViewBag.planID = new SelectList(db.Plans, "planID", "planID");
            return View();
        }

        // GET: Users/Login
        public ActionResult Login()
        {
            return View();
        }

        // GET: Users/Logout
        public ActionResult Logout()
        {
            System.Web.HttpContext.Current.Session.Clear();
            return Redirect("~/Home/Index");
        }

        // GET: Users/GetIn
        public ActionResult GetIn([Bind(Include = "email,phoneNumber,password")]string phoneNumber,string password)
        {
            var check = db.Users.Where(t=>(t.phoneNumber==phoneNumber||t.email==phoneNumber)&& t.password==password).FirstOrDefault();
            if (check != null)
            {

                System.Web.HttpContext.Current.Session.Add("UserId", check.userID);
            }
            return Redirect("~/Home/Index");
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "userID,planID,firstName,lastName,SSN,balance,email,phoneNumber,password,loginStatus")] User user)
        {
            if (user.SSN != null)
            {
                user.planID = 1;
            }

            else
            {
                user.planID = 2;
            }
            //if (ModelState.IsValid)
            //{
            user.balance = 0;
            db.Users.Add(user);
            db.SaveChanges();
        
            return Redirect("~/Home/Index");
            //}
            ViewBag.planID = new SelectList(db.Plans, "planID", "planID", user.planID);
            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.planID = new SelectList(db.Plans, "planID", "planID", user.planID);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "userID,planID,firstName,lastName,SSN,balance,email,phoneNumber,password,loginStatus")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.planID = new SelectList(db.Plans, "planID", "planID", user.planID);
            return Redirect("~/Home/Index");
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
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
