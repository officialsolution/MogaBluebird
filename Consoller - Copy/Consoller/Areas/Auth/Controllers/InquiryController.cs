using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Consoller.Areas.Auth.Models;
using onlineportal.Areas.AdminPanel.Models;

namespace Consoller.Areas.Auth.Controllers
{
    public class InquiryController : Controller
    {
        private dbcontext db = new dbcontext();
        Helper help = new Helper();
        SQLHelper objsql = new SQLHelper();
        // GET: Auth/Inquiry
        public ActionResult Index()
        {
            if (User.IsInRole("Franchisee"))
            {
                string franch = help.Franchisee();
                return View(db.tblinquiries.Where(x => x.franchid == franch).ToList());

            }
            else if (User.IsInRole("Receptionist"))
            {
                string franch = help.Receptionist();
                return View(db.tblinquiries.Where(x => x.franchid == franch).ToList());

            }
            else if (User.IsInRole("Admin"))
            {
                return View(db.tblinquiries.ToList());
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            //var category = db.Categories.ToList();
            // return View(db.tblinquiries.ToList());
        }

        // GET: Auth/Inquiry/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblinquiry tblinquiry = db.tblinquiries.Find(id);
            ViewBag.Categoryid = new SelectList(db.Categories, "Categoryid", "Name");
            if (tblinquiry == null)
            {
                return HttpNotFound();
            }
            return View(tblinquiry);
        }

        // GET: Auth/Inquiry/Create
        [Authorize(Roles = "Franchisee,Receptionist")]
        public ActionResult Create()
        {
            tblinquiry inquiry = new tblinquiry();
            inquiry.date = System.DateTime.Now;
            ViewBag.Categoryid = new SelectList(db.Categories, "Categoryid", "Name");
            inquiry.status = true;
            return View(inquiry);
        }

        // POST: Auth/Inquiry/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,date,inquiryid,name,fname,contact,address,referedby,Categoryid,Type")] tblinquiry tblinquiry, [Bind(Include = "Id,date,inquiryid,feedback,days,type,nexfollow,status,loginid")] tblfeedback tblfeedback, Helper Help)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                {
                    try
                    {
                        if (ModelState.IsValid)
                        {
                            tblinquiry.status = true;
                            tblinquiry.Type = "Offline";
                            string a = User.IsInRole("Franchisee") ? help.Franchisee() : help.Receptionist();
                            tblinquiry inquiry = db.tblinquiries.FirstOrDefault();
                            if (inquiry == null)
                            {
                                tblinquiry.inquiryid = "101";
                            }
                            else
                            {
                                //  var ab = db.tblinquiries.Where(x => x.status == true && x.franchid=a).Max(x => x.inquiryid);
                                string max = objsql.GetSingleValue("select inquiryid from tblinquiries where  franchid='" + a + "' and id=(select max(id) from tblinquiries)").ToString();
                                string neww = (Convert.ToInt32(max) + Convert.ToInt32(1)).ToString();
                                tblinquiry.inquiryid = neww;
                            }
                            tblinquiry.franchid = a;

                            db.tblinquiries.Add(tblinquiry);
                            db.SaveChanges();
                            tblfeedback.date = tblinquiry.date;
                            tblfeedback.inquiryid = tblinquiry.inquiryid;
                            tblfeedback.nextfollow = tblinquiry.date.AddDays(tblfeedback.days);
                            tblfeedback.status = tblinquiry.status;
                            tblfeedback.loginid = help.Permission();
                            db.tblfeedback.Add(tblfeedback);
                            db.SaveChanges();


                            transaction.Commit();

                            TempData["Success"] = "Saved Successfully";
                            tblsms sms = db.tblsms.FirstOrDefault();
                            if (tblinquiry.contact != null)
                            {
                                if (sms != null)
                                {
                                    string msg = "Dear " + Convert.ToString(tblinquiry.name) + ". It was a pleasure to meet you, and I’m so glad we got a chance to discuss what you’re looking. Thanks for your visit, and I hope to see you soon!";
                                    string result = Help.apicall("http://sms.sms.officialsolutions.in/sendSMS?username=" + sms.Username + "&message=" + msg + "&sendername=" + sms.Senderid + "&smstype=TRANS&numbers=" + tblinquiry.contact + "&apikey=" + sms.Api + "");

                                    TempData["Success"] = "SMS Send Successfully";
                                }
                            }
                            ViewBag.Categoryid = new SelectList(db.Categories, "Categoryid", "Name");
                            return RedirectToAction("Index");
                       
                        }
                        ViewBag.Categoryid = new SelectList(db.Categories, "Categoryid", "Name");
                        return View(tblinquiry);
                    }
                    catch (DbEntityValidationException e)
                    {
                        transaction.Dispose();
                        transaction.Rollback();

                        foreach (var eve in e.EntityValidationErrors)
                        {
                            Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                eve.Entry.Entity.GetType().Name, eve.Entry.State);
                            foreach (var ve in eve.ValidationErrors)
                            {
                                Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                    ve.PropertyName, ve.ErrorMessage);
                            }
                        }
                        throw;
                    }
                }
            }
        }

        // GET: Auth/Inquiry/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblinquiry tblinquiry = db.tblinquiries.Find(id);
            ViewBag.Categoryid = new SelectList(db.Categories, "Categoryid", "Name",tblinquiry.Categoryid);
            tblfeedback tbfeedback = db.tblfeedback.FirstOrDefault(x => x.inquiryid == tblinquiry.inquiryid);

            TempData["feedback"] = tblinquiry.inquiryid;
            if (tblinquiry == null)
            {
                return HttpNotFound();
            }
            return View(tblinquiry);
        }

        // POST: Auth/Inquiry/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,date,inquiryid,name,fname,contact,address,referedby,Categoryid,Type,Status")] tblinquiry tblinquiry, [Bind(Include = "Id,date,inquiryid,feedback,days,type,nexfollow,status,loginid")] tblfeedback tblfeedback)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //db.Entry(tblinquiry).State = EntityState.Modified;
                    //db.SaveChanges();
                    tblfeedback ff = db.tblfeedback.FirstOrDefault(x => x.inquiryid == tblinquiry.inquiryid);
                    if(ff!=null)
                    { 
                    DateTime next = new DateTime();
                    if (tblfeedback.type == "Days")
                    {
                        next = System.DateTime.Now.AddDays(tblfeedback.days);

                    }

                    else
                    {
                        next = System.DateTime.Now;
                    }
                     objsql.ExecuteNonQuery("update tblfeedbacks set date='" + tblinquiry.date + "',nextfollow='" + next + "' where id='" + ff.Id + "'");
                    TempData["feedback"] = tblinquiry.inquiryid;
                   
                        TempData["Success"] = "Updated Successfully";
                    return RedirectToAction("Index");
                        }
                }
                return View(tblinquiry);
            }
            catch (DbEntityValidationException e)
            {

                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
        }

        // GET: Auth/Inquiry/Delete/5
        [Authorize(Roles = "Franchisee")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblinquiry tblinquiry = db.tblinquiries.Find(id);
            ViewBag.Categoryid = new SelectList(db.Categories, "Categoryid", "Name");
            if (tblinquiry == null)
            {
                return HttpNotFound();
            }
            return View(tblinquiry);
        }

        // POST: Auth/Inquiry/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SQLHelper objsql = new SQLHelper();
            tblinquiry tblinquiry = db.tblinquiries.Find(id);
            ViewBag.Categoryid = new SelectList(db.Categories, "Categoryid", "Name");

            objsql.ExecuteNonQuery("delete from tblfeedbacks where inquiryid='" + tblinquiry.inquiryid + "'");
            db.tblinquiries.Remove(tblinquiry);

            db.SaveChanges();

            TempData["Success"] = "Deleted Successfully";
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
        public ActionResult Assign(int id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            //online online = db.onlines.FirstOrDefault(x => x.inquiryid == id);
            //if (online != null)
            //{
            //    return PartialView("~/Areas/Auth/Views/onlines/AssignUpdate.cshtml", online);
            //}
            //else
            //{
            //    online on = db.onlines.FirstOrDefault(x => x.inquiryid == id);
            //    return PartialView("~/Areas/Auth/Views/onlines/Assign.cshtml", on);
            //}
            string a = id.ToString();
            tblinquiry on = db.tblinquiries.FirstOrDefault(x => x.inquiryid == a && x.status == true);
            return PartialView("~/Areas/Auth/Views/Inquiry/Assign.cshtml", on);

        }
        public ActionResult ReAssign(int id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            string franchid = help.Permission();
            string a = id.ToString();
            tblinquiry online = db.tblinquiries.FirstOrDefault(x => x.inquiryid == a && x.franchid == franchid && x.status == true);
            return PartialView("~/Areas/Auth/Views/Inquiry/AssignUpdate.cshtml", online);


        }
        public ActionResult UpdateAssign([Bind(Include = "inquiryid")] tblinquiry online, string assign, AssignIelts assign1, Helper help)
        {

            assign1.Date = System.DateTime.Now;
            assign1.inquiryid = Convert.ToInt32(online.inquiryid);
            assign1.teacher = assign;
            assign1.franchid = help.Permission();
            assign1.Status = true;
            db.AssignIelts.Add(assign1);
            db.SaveChanges();
            var result = db.tblinquiries.SingleOrDefault(b => b.inquiryid == online.inquiryid && b.franchid == assign1.franchid && b.status == true);
            if (result != null)
            {
                result.teacher = assign;
                db.SaveChanges();
            }
            TempData["Success"] = "Assign Inquiry Successfully";

            return RedirectToAction("Index", "Auth/Inquiry");
        }
        public ActionResult UpdateAssignTeacher([Bind(Include = "inquiryid")] tblinquiry online, string assign, AssignIelts assign1, Helper help, tblreceptionist tblreceptionist)
        {

            SQLHelper objsql = new SQLHelper();
            objsql.ExecuteNonQuery("update AssignIelts set status='0' where inquiryid='" + assign1.inquiryid + "'");

            assign1.Date = System.DateTime.Now;
            assign1.inquiryid = Convert.ToInt32(online.inquiryid);
            assign1.teacher = assign1.teacher;
            assign1.franchid = help.Permission();
            assign1.Status = true;
            db.AssignIelts.Add(assign1);
            db.SaveChanges();
            var result1 = db.tblinquiries.SingleOrDefault(b => b.inquiryid == online.inquiryid && b.franchid == assign1.franchid);
            if (result1 != null)
            {
                result1.teacher = assign1.teacher;
                db.SaveChanges();
            }
            TempData["Success"] = "Update Lead Successfully";

            return RedirectToAction("Index", "Auth/Inquiry");
        }
    }
}
