using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FitCen.Models;

namespace FitCen.Controllers
{
    public class ReviewController : Controller
    {
        // GET: Review
        public ActionResult Index(int id)
        {
            ViewBag.tid = id;
            return View();
        }

        // GET: Review/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Review/Create
        public ActionResult Create()
        {
            return View("Index");
        }

        // POST: Review/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                int uid = Convert.ToInt32(Session["uid"]);
                int tid = Convert.ToInt32(collection["training_id"]);
                int review = Convert.ToInt32(collection["review"]);
                string comment = collection["comment"];

                Review r = new Review(uid, tid, review, comment);
                r.id = Review.GetID();
                if (r.create() == 1)
                {
                    TempData["success"] = "Uspesno ocenjen trening";
                }
                else
                {
                    TempData["error"] = "Doslo je do greske prilikom ocenjivanja treninga";
                }
                return RedirectToAction("Index", "User");
            }
            catch
            {
                return View();
            }
        }

        // GET: Review/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Review/Edit/5
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

        // GET: Review/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Review/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        public ActionResult Accept(int id)
        {
            Review r = Review.GetReview(id);
            int uid = Convert.ToInt32(Session["uid"]);
            Training t = Training.GetTraining(r.training_id);
            FitnessCenter c = FitnessCenter.GetCenter(t.center_id);
            if (c.owner_id != uid)
            {
                TempData["error"] = "Nemate kontrolu nad ovim centrom";
                return RedirectToAction("Index", "User");

            }
            if (r.enableReview() == 1)
            {
                TempData["success"] = "Ocena uspesno odobrena";
            }
            else
            {
                TempData["error"] = "Doslo je do greske prilikom odobravanja ocene";
            }
            return RedirectToAction("Index", "User");
        }
        public ActionResult Reject(int id)
        {
            Review r = Review.GetReview(id);
            int uid = Convert.ToInt32(Session["uid"]);
            Training t = Training.GetTraining(r.training_id);
            FitnessCenter c = FitnessCenter.GetCenter(t.center_id);
            if (c.owner_id != uid)
            {
                TempData["error"] = "Nemate kontrolu nad ovim centrom";
                return RedirectToAction("Index", "User");

            }
            if (r.disableReview() == 1)
            {
                TempData["success"] = "Ocena uspesno odbijena";
            }
            else
            {
                TempData["error"] = "Doslo je do greske prilikom odobravanja ocene";
            }
            return RedirectToAction("Index", "User");
        }
        public ActionResult GetCenterReviews(int id)
        {
            Training training = Training.GetTraining(id);
            int center_id = training.center_id;
            List<dynamic> reviews = Review.GetReviewsForCenter(center_id);
            ViewBag.reviews = reviews;
            return View();
        }
    }
}
