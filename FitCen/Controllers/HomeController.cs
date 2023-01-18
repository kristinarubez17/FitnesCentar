using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using FitCen.Models;

namespace FitCen.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            List<FitnessCenter> centers = FitnessCenter.GetCenters();
            ViewBag.centers = centers;
            return View();
        }
        public ActionResult Search()
        {

            List<FitnessCenter> centers = FitnessCenter.GetCenters();
            string searchText = Request.QueryString["search"];
            int year_upper = (Request.QueryString["year_upper"] != "") ? Convert.ToInt32(Request.QueryString["year_upper"]) : int.MaxValue;
            int year_lower = (Request.QueryString["year_lower"] != "") ? Convert.ToInt32(Request.QueryString["year_lower"]) : int.MinValue;
            string sorting = Request.QueryString["sorting"];
            string sorting_order = Request.QueryString["sorting_order"];
            List<FitnessCenter> tr = new List<FitnessCenter>();
            foreach (var t in centers)
            {
                if (t.name.Contains(searchText) || t.address.Contains(searchText))
                {
                    tr.Add(t);
                }
            }
            List<FitnessCenter> sp = new List<FitnessCenter>();
            foreach (var t in tr)
            {
                if (t.year_opened > year_lower && t.year_opened < year_upper)
                {
                    sp.Add(t);
                }
            }
            if (tr.Count != 0)
            {
                if (sorting != "Sortiranje")
                {
                    if (sorting == "name")
                    {
                        sp.OrderBy(t => t.name);
                    }
                    else if (sorting == "address")
                    {
                        sp.OrderBy(t => t.address);
                    }
                    else if (sorting == "year")
                    {
                        sp.OrderBy(t => t.year_opened);
                    }
                }
                if (sorting_order != "Poredak")
                {
                    if (sorting_order == "ascending")
                    {

                    }
                    else if (sorting_order == "descending")
                    {
                        sp.Reverse();
                    }
                }
                ViewBag.centers = sp;
            }
            else
            {
                ViewBag.centers = centers;
            }
            ViewBag.search = searchText;
            ViewBag.year_lower = (year_lower == int.MinValue) ? 0 : year_lower;
            ViewBag.year_upper = (year_upper == int.MaxValue) ? 0 : year_upper;
            ViewBag.sorting = sorting;
            ViewBag.sorting_order = sorting_order;
            return View("Index");
        }
    }
}