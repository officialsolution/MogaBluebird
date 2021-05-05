using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Consoller.Areas.Auth.Models;

namespace Consoller.Areas.Admin.Controllers
{
    public class NotificationsController : Controller
    {
        private dbcontext db = new dbcontext();

        // GET: Admin/Notifications
        public async Task<ActionResult> Index()
        {
            return View(await db.Notifications.ToListAsync());
        }

        // GET: Admin/Notifications/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = await db.Notifications.FindAsync(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            return View(notification);
        }

        // GET: Admin/Notifications/Create
        public ActionResult Create()
        {
            Notification no = new Notification();
            no.Status = true;

            return View(no);
        }

        // POST: Admin/Notifications/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Date,Message,Status,Priority,Consoller")] Notification notification, string[] take)
        {
            if (ModelState.IsValid)
            {
                if (take.Length > 0)
                {
                    for (int i = 0; i <= take.Length - 1; i++)
                    {
                        notification.Consoller = take[i];
                        db.Notifications.Add(notification);
                        await db.SaveChangesAsync();
                    }
                    return RedirectToAction("Index");
                }
                else
                {

                    TempData["danger"] = "Please Select User";
                    return View();
                }
            }

            return View(notification);
        }

        // GET: Admin/Notifications/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = await db.Notifications.FindAsync(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            return View(notification);
        }

        // POST: Admin/Notifications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Date,Message,Status,Priority,Consoller")] Notification notification, string[] take)
        {
            if (ModelState.IsValid)
            {
                db.Entry(notification).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(notification);
        }

        // GET: Admin/Notifications/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = await db.Notifications.FindAsync(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            return View(notification);
        }

        // POST: Admin/Notifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Notification notification = await db.Notifications.FindAsync(id);
            db.Notifications.Remove(notification);
            await db.SaveChangesAsync();
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
