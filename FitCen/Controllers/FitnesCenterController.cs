using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FitCen.Models;
namespace FitCen.Controllers
{
    public class FitnesCenterController : Controller
    {
        // GET: FitnesCenter
        public ActionResult Index()
        {
            return View();
        }

        // GET: FitnesCenter/Details/5
        public ActionResult Details(int id)
        {
            FitnessCenter Model = FitnessCenter.GetCenter(id);
            List<dynamic> trainings = Training.GetTrainings(id);
            List<dynamic> ratings = Review.GetReviewsForCenter(id);
            ratings.RemoveAll(r => r.display == 0);
            trainings.RemoveAll(t => DateTime.Compare(t.start, DateTime.Now) < 0);

            ViewBag.trainings = trainings;
            ViewBag.ratings = ratings;


            return View(Model);
        }

        // GET: FitnesCenter/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FitnesCenter/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                string name = collection["name"];
                string address = collection["address"];
                int year_opened = Convert.ToInt32(collection["year_opened"]);
                int owner_id = Convert.ToInt32(Session["uid"]);
                int month_price = Convert.ToInt32(collection["month_price"]);
                int year_price = Convert.ToInt32(collection["year_price"]);
                int group_price = Convert.ToInt32(collection["group_price"]);
                int group_price_with_trainer = Convert.ToInt32(collection["group_price_with_trainer"]);
                FitnessCenter instance = new FitnessCenter(name, address, year_opened, owner_id, month_price, year_price, group_price, group_price_with_trainer);
                instance.id = FitnessCenter.getID();
                if (instance.create() == 1)
                {
                    TempData["success"] = "Uspesno kreiran fitnes centar";
                }
                else
                {
                    TempData["error"] = "Doslo je do greske prilikom kreiranja fitnes centra";
                }

                return RedirectToAction("Index", "User");
            }
            catch
            {
                return View();
            }
        }

        // GET: FitnesCenter/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: FitnesCenter/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: FitnesCenter/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: FitnesCenter/Delete/5
        [HttpPost]
        public ActionResult Delete(FormCollection collection)
        {
            try
            {
                FitnessCenter f = FitnessCenter.GetCenter(Convert.ToInt32(collection["center_id"]));
                int uid = Convert.ToInt32(Session["uid"]);
                if (f.owner_id != uid)
                {
                    TempData["error"] = "Nemate pristup ovom fitnes centru";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    if (f.delete() == 1)
                    {
                        TempData["success"] = "Uspesno uklonjen fitnes centar";
                    }
                    else
                    {
                        TempData["error"] = "Ne mozete obrisati ovaj fitnes centar";
                    }
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                return View();
            }
        }
    }
}
