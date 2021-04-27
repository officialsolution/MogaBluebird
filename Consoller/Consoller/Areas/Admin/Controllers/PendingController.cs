using Consoller.Areas.Auth.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Consoller.Areas.Admin.Controllers
{
    public class PendingController : Controller
    {
        dbcontext db = new dbcontext();
        // GET: Admin/Pending
        public ActionResult GicPending()
        {
            return View(db.Process.Where(x=>x.Gic==false));
        }
        public ActionResult OfferPending()
        {
            return View(db.Process.Where(x => x.Offerletter == false));
        }
        public ActionResult MedicalPending()
        {
            return View(db.Process.Where(x => x.Medical == false));
        }
        public ActionResult SubmitPending()
        {
            return View(db.Process.Where(x => x.SubmitFile == false));
        }
    }
}