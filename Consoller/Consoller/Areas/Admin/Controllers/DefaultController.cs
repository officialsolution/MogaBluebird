﻿using Consoller.Areas.Admin.Models;
using Consoller.Areas.Auth.Models;
using Consoller.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Consoller.Areas.Admin.Controllers
{
    public class DefaultController : Controller
    {
        // GET: Admin/Default

        AdminData ad = new AdminData();
        dbcontext db = new dbcontext();
        SQLHelper objsql = new SQLHelper();
        Helper help = new Helper();

        public ActionResult Dashaboard()
        {
            string a = help.Permission();
            if (a=="Admin")
            {
                List<DataPoint> dataPoints = new List<DataPoint>();
                int offer = Convert.ToInt32(objsql.GetSingleValue("select count(*) from processes where Offerletter=0"));
                int Gic = Convert.ToInt32(objsql.GetSingleValue("select count(*) from processes where Gic=0"));
                int Medical = Convert.ToInt32(objsql.GetSingleValue("select count(*) from processes where Medical=0"));
                var submit = Convert.ToInt32(objsql.GetSingleValue("select count(*) from processes where SubmitFile=0"));
                dataPoints.Add(new DataPoint("Offer Letter", offer));
                dataPoints.Add(new DataPoint("Gic", Gic));
                dataPoints.Add(new DataPoint("Medical", Medical));
                dataPoints.Add(new DataPoint("Submit", submit));
                ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);

                List<ExpenseReport> Reports = new List<ExpenseReport>();
                DataTable dt = new DataTable();
                // DateTime todaydate = System.DateTime.Now.ToShortDateString();
                int expense = Convert.ToInt32(objsql.GetSingleValue("select COALESCE(sum(amount),0) from expenses where date='" + System.DateTime.Now.ToShortDateString() + "'"));
                int immfee = Convert.ToInt32(objsql.GetSingleValue("select COALESCE(sum(amount),0) from ApplicationRecipts where date='" + System.DateTime.Now.ToShortDateString() + "'"));
                int ieltsfee = Convert.ToInt32(objsql.GetSingleValue("select COALESCE(sum(amount),0) from Recipt_Details where date='" + System.DateTime.Now.ToShortDateString() + "'"));

                //var immfee = db.ApplicationRecipts.Where(x => x.Date == System.DateTime.Now.ToShortDateString()).Select(x=>x.Amount).DefaultIfEmpty(0).Sum();
                //var ieltsfee = db.Recipt_Details.Where(x => x.Date == System.DateTime.Now).Select(x=>x.Amount).DefaultIfEmpty(0).Sum();
                int total = (Convert.ToInt32(immfee) + Convert.ToInt32(ieltsfee));
                double profit = total - expense;
                Reports.Add(new ExpenseReport("Expense", expense));
                Reports.Add(new ExpenseReport("Total", total));
                Reports.Add(new ExpenseReport("Profit", profit));

                ViewBag.Report = JsonConvert.SerializeObject(Reports);
                return View(ad); 
            }
            else
            {
                List<DataPoint> dataPoints = new List<DataPoint>();
                int offer = Convert.ToInt32(objsql.GetSingleValue("select count(*) from processes where Offerletter=0 and franchid='"+a+"'"));
                int Gic = Convert.ToInt32(objsql.GetSingleValue("select count(*) from processes where Gic=0 and franchid='" + a + "'"));
                int Medical = Convert.ToInt32(objsql.GetSingleValue("select count(*) from processes where Medical=0 and franchid='" + a + "'"));
                var submit = Convert.ToInt32(objsql.GetSingleValue("select count(*) from processes where SubmitFile=0 and franchid='" + a + "'"));
                dataPoints.Add(new DataPoint("Offer Letter", offer));
                dataPoints.Add(new DataPoint("Gic", Gic));
                dataPoints.Add(new DataPoint("Medical", Medical));
                dataPoints.Add(new DataPoint("Submit", submit));
                ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);

                List<ExpenseReport> Reports = new List<ExpenseReport>();
                DataTable dt = new DataTable();
                // DateTime todaydate = System.DateTime.Now.ToShortDateString();
                int expense = Convert.ToInt32(objsql.GetSingleValue("select COALESCE(sum(amount),0) from expenses where date='" + System.DateTime.Now.ToShortDateString() + "' and franchid='" + a + "'"));
                int immfee = Convert.ToInt32(objsql.GetSingleValue("select COALESCE(sum(amount),0) from ApplicationRecipts where date='" + System.DateTime.Now.ToShortDateString() + "' and franchid='" + a + "'"));
                int ieltsfee = Convert.ToInt32(objsql.GetSingleValue("select COALESCE(sum(amount),0) from Recipt_Details where date='" + System.DateTime.Now.ToShortDateString() + "' and franchid='" + a + "'"));

                //var immfee = db.ApplicationRecipts.Where(x => x.Date == System.DateTime.Now.ToShortDateString()).Select(x=>x.Amount).DefaultIfEmpty(0).Sum();
                //var ieltsfee = db.Recipt_Details.Where(x => x.Date == System.DateTime.Now).Select(x=>x.Amount).DefaultIfEmpty(0).Sum();
                int total = (Convert.ToInt32(immfee) + Convert.ToInt32(ieltsfee));
                double profit = total - expense;
                Reports.Add(new ExpenseReport("Expense", expense));
                Reports.Add(new ExpenseReport("Total", total));
                Reports.Add(new ExpenseReport("Profit", profit));

                ViewBag.Report = JsonConvert.SerializeObject(Reports);
                return View(ad);
            }
        }
        public ActionResult Details(int id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            online online = db.onlines.FirstOrDefault(x => x.inquiryid == id);
            return PartialView("~/Areas/Admin/Views/Default/InquiryDetails.cshtml", online);

        }
        public ActionResult Data(int id)
        {

            return View();

        }
        public ActionResult AllApplication()
        {
            string role = help.Permission();

            if(role=="Admin")
            {
                return View(db.Applications.OrderByDescending(x => x.Year).ToList());
            }
            else
            {
               
                return View(db.Applications.Where(x=>x.Franchid==role).OrderByDescending(x => x.Year).ToList());
            }
           
        }
        public ActionResult Application()
        {
            string role = help.Permission();
            if(role=="Admin")
            {
                return View(db.onlines.OrderByDescending(x => x.Date).ToList());
            }
            else
            {
                return View(db.onlines.Where(x=>x.franchid==role).OrderByDescending(x => x.Date).ToList());
            }
            
        }
        public ActionResult Profile(int id)
        {
            return View(db.Applications.FirstOrDefault(x=>x.ApplicationNo==id));
        }
        public ActionResult AllPayment()
        {
            string role = help.Permission();
            if (role == "Admin")
            {
                return View(db.ApplicationRecipts.ToList());
            }
            else
            {
                return View(db.ApplicationRecipts.Where(x=>x.Franchid==role).ToList());
            }
        }
        public ActionResult Expense()
        {

            string role = help.Permission();
            if (role == "Admin")
            {
                return View(db.Expenses.ToList());
            }
            else
            {
                return View(db.Expenses.Where(x=>x.franchid==role).ToList());
            }
        }
        public ActionResult DailyReports()
        {
            DashModel dd = new DashModel();
            return View(dd);

        }
        [HttpGet]
        public ActionResult DateWise()
        {
            Session["Status"] = "False";
            return View();

        }
        [HttpPost]
        public ActionResult DateWise(Expense ex)
        {
            Session["Status"] = "True";
            dynamic mymodel = new ExpandoObject();
            mymodel.expense = db.Expenses.Where(x => x.Date >= ex.Date1 && x.Date <= ex.Date2);
            mymodel.dailyinquiry = db.tblinquiries.Where(x => x.date >= ex.Date1 && x.date <= ex.Date2);
            mymodel.immgrationinquiry = db.onlines.Where(x => x.Date >= ex.Date1 && x.Date <= ex.Date2);
            mymodel.ieltsfee = db.Recipt_Details.Where(x => x.Date >= ex.Date1 && x.Date <= ex.Date2);
            mymodel.ConfirmCase = db.Applications.Where(x => x.Date >= ex.Date1 && x.Date <= ex.Date2);
            mymodel.CasePayment = db.ApplicationRecipts.Where(x => x.Date >= ex.Date1 && x.Date <= ex.Date2);
            return View(mymodel);

        }
        [HttpGet]
        public ActionResult ExpenseReport()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ExpenseReport(Expense ex)
        {
            return View(db.Expenses.Where(x => x.Date >=ex.Date1 && x.Date<=ex.Date2).ToList());
        }
    }
}