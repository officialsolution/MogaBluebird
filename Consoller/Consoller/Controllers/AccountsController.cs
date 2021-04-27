using Consoller.Areas.Auth.Models;
using Consoller.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Consoller.Controllers
{
    public class AccountsController : Controller
    {
        // GET: Accounts
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(tblreceptionist model, string returnUrl, Helper Help, string ddltype)
        {
            dbcontext db = new dbcontext();

            var otp = Help.otp();
          
            tblreceptionist rr = db.tblreceptionists.FirstOrDefault(x => x.rid == model.rid);
            tbldetail dd = db.tbldetails.FirstOrDefault();
              Help.sendsms(rr.contact, "Dear, " + rr.name + ". Login OTP Is " + otp+ " ");
            tblreceptionist data = new tblreceptionist()
            {
                Type = ddltype,
                rid = model.rid,
                OTP = Convert.ToInt32(otp)
            };
            Session["mydata"] = data;
            return RedirectToAction("OTP", "Accounts");
        }
        public ActionResult ChangeRole(tblreceptionist model, string returnUrl, Helper Help, int id)
        {
            dbcontext db = new dbcontext();
            var passw = model.password;
            string ids = id.ToString();
            var dataItem = db.tblreceptionists.Where(x => x.rid == ids).FirstOrDefault();
            if (dataItem != null)
            {

                FormsAuthentication.SetAuthCookie(dataItem.rid, false);

                if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                         && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    TempData["Success"] = "Login Successfully";
                    Session["User"] = dataItem.Id;
                    Session["admin"] = dataItem.Id;
                    return RedirectToAction("Index", "Admin/Dashaboard");


                }

            }
            else
            {
                // ModelState.AddModelError("", "Invalid user/pass");
                TempData["Success"] = "Invalid user/pass";
                return View();
            }
        }
        //[Authorize]
        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }
        [HttpGet]
        public ActionResult OTP()
        {
            return View();
        }
        [HttpPost]
        public ActionResult OTP(tblreceptionist recp, string returnUrl, Helper Help, string ddltype)
        {
            
            dbcontext db = new dbcontext();
            tblreceptionist data = Session["mydata"] as tblreceptionist;
            string login = data.rid;
            int otp = data.OTP;
            ddltype = data.Type;
            //string login = TempData["Rid"].ToString();
            if(otp==recp.OTP)
            {
                var dataItem = db.tblreceptionists.Where(x => x.rid == login).FirstOrDefault();
                if (dataItem != null)
                {

                    FormsAuthentication.SetAuthCookie(dataItem.rid, false);

                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                             && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        TempData["danger"] = "Invalid user/pass";
                        return Redirect(returnUrl);
                    }
                    else if (ddltype == "Processing")
                    {
                        TempData["Success"] = "Login Successfully";
                        return RedirectToAction("AllApplication", "Processing");
                    }
                    else if (ddltype == "Gic")
                    {
                        TempData["Success"] = "Login Successfully";
                        return RedirectToAction("AllApplication", "Processing");
                    }
                    else if (ddltype == "Consoller")
                    {
                        TempData["Success"] = "Login Successfully";
                        return RedirectToAction("Index", "Consoller");
                    }
                    else if (ddltype == "Chandigarh")
                    {
                        TempData["Success"] = "Login Successfully";
                        return RedirectToAction("AllApplication", "Processing");
                    }
                    else if (ddltype == "Admin")
                    {
                        TempData["Success"] = "Login Successfully";
                        return RedirectToAction("Dashaboard", "Admin/Default");
                    }
                    else
                    {
                        TempData["Success"] = "Login Successfully";
                        Session["User"] = dataItem.Id;

                        Session["Franchisee"] = dataItem.franchid;
                        return RedirectToAction("Index", "Auth/Default");

                    }
                }
                else
                {
                    // ModelState.AddModelError("", "Invalid user/pass");
                    TempData["danger"] = "Invalid user/pass";
                    return View();
                }
            }
            else
            {
                // ModelState.AddModelError("", "Invalid user/pass");
                TempData["danger"] = "Invalid OTP";
                return View();
            }
          
            
        }
        public ActionResult Login2(tblreceptionist model, string returnUrl, Helper Help, string ddltype)
        {
            dbcontext db = new dbcontext();
            var passw = model.password;
            var dataItem = db.tblreceptionists.Where(x => x.login == model.login && x.password == passw && x.Type == ddltype).FirstOrDefault();
            if (dataItem != null)
            {

                FormsAuthentication.SetAuthCookie(dataItem.rid, false);

                if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                         && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                {
                    TempData["danger"] = "Invalid user/pass";
                    return Redirect(returnUrl);
                }
                else if (ddltype == "Processing")
                {
                    TempData["Success"] = "Login Successfully";
                    return RedirectToAction("AllApplication", "Processing");
                }
                else if (ddltype == "Gic")
                {
                    TempData["Success"] = "Login Successfully";
                    return RedirectToAction("AllApplication", "Processing");
                }
                else if (ddltype == "Consoller")
                {
                    TempData["Success"] = "Login Successfully";
                    return RedirectToAction("Index", "Consoller");
                }
                else if (ddltype == "Chandigarh")
                {
                    TempData["Success"] = "Login Successfully";
                    return RedirectToAction("AllApplication", "Processing");
                }
                else if (ddltype == "Admin")
                {
                    TempData["Success"] = "Login Successfully";
                    return RedirectToAction("Dashaboard", "Admin/Default");
                }
                else
                {
                    TempData["Success"] = "Login Successfully";
                    Session["User"] = dataItem.Id;

                    Session["Franchisee"] = dataItem.franchid;
                    return RedirectToAction("Index", "Auth/Default");

                }
            }
            else
            {
                // ModelState.AddModelError("", "Invalid user/pass");
                TempData["danger"] = "Invalid user/pass";
                return View();
            }
        }
    }

}