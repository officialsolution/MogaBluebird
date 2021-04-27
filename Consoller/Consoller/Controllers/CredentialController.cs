﻿using Consoller.Areas.Auth.Models;
using Consoller.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Consoller.Controllers
{
    public class CredentialController : Controller
    {
        // GET: Credential
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
                    return RedirectToAction("Index", "Admin/Default");
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

        //[Authorize]
        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Credential");
        }
        public ActionResult Login2()
        {
            return View();
        }

        [HttpPost]
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
                    return RedirectToAction("Index", "Admin/Default");
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
