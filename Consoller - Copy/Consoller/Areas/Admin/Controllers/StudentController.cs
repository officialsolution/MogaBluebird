using Consoller.Areas.Auth.Models;
using Consoller.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Consoller.Areas.Admin.Controllers
{
    public class StudentController : Controller
    {
        Helper help = new Helper();
        dbcontext db = new dbcontext();
        // GET: Admin/Student
        public ActionResult Index()
        {
            string role = help.Permission();
            if (User.IsInRole("Admin"))
            {
                var studentdata = db.tblstudentdata.Where(x => x.Status == true).ToList();
                return View(studentdata);
            }
            else
            {
                var studentdata = db.tblstudentdata.Where(x => x.Status == true && x.uid == role).ToList();
                return View(studentdata);

            }
        }
        public ActionResult Logs(int roll)
        {
            tblstudentdata ss = db.tblstudentdata.FirstOrDefault(x => x.rollno == roll);
            TempData["roll"] = roll;
            Session["roll"] = roll;
            Session["franch"] = ss.uid;
            var recp = db.Recipt_Details.Where(x => x.RollNo == roll && x.franchid == ss.uid).ToList();
            return View(recp);
        }
    }
}