using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using FitCen.Models;

namespace FitCen.Controllers
{
    public class TrainingController : Controller
    {
        // GET: Training
        public ActionResult Index()
        {
            return View();
        }

        // GET: Training/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Training/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Training/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                string name = collection["name"];
                int duration = Convert.ToInt32(collection["duration"]);
                DateTime start = Convert.ToDateTime(collection["start"]);
                if ((start - DateTime.Now).TotalDays < 3)
                {
                    TempData["error"] = "Trening se moze kreirati najranije 3 dana pred termin";
                    return RedirectToAction("Index", "User");
                }
                int max_people = Convert.ToInt32(collection["max_people"]);
                string type = collection["training_type"];
                Training training = new Training(name, type, duration, start, max_people);
                training.id = Training.getID();
                User logged = FitCen.Models.User.GetUser(Convert.ToInt32(Session["uid"]));
                training.center_id = logged.center_id;
                training.trainer_id = logged.id;
                if (training.create() == 1)
                {
                    TempData["success"] = "Uspesno kreiran trening";
                }
                else
                {
                    TempData["error"] = "Doslo je do greske prilikom kreiranja treninga";
                }
                return RedirectToAction("Index", "User");
            }
            catch (Exception e)
            {
                TempData["error"] = "Doslo je do greske prilikom kreiranja treninga. Greska: " + e.Message;

                return RedirectToAction("Index", "User");
            }
        }

        // GET: Training/Edit/5
        public ActionResult Edit(int id)
        {
            Training model = Training.GetTraining(id);
            ViewBag.training = model;
            return View();
        }

        // POST: Training/Edit/5
        [HttpPost]
        public ActionResult Edit(FormCollection collection)
        {
            try
            {
                int id = Convert.ToInt32(collection["training_id"]);
                Training t = Training.GetTraining(id);
                t.max_people = Convert.ToInt32(collection["max_people"]);
                t.name = collection["name"];
                DateTime prevStart = t.start;
                t.start = Convert.ToDateTime(collection["start"]);
                t.training_type = collection["training_type"];
                List<Appointment> apps = Appointment.GetTrainingAppointments(t.id);
                if(apps.Count > 0 || DateTime.Compare(prevStart,t.start) < 0)
                {
                    TempData["error"] = "Ne mozete azurirati ovaj trening";
                    return RedirectToAction("Index","Home");

                }
                if (t.update() == 1) {
                    TempData["success"] = "Uspesno azuriran trening";
                }
                else
                {
                    TempData["error"] = "Doslo je do greske prilikom azuriranja treninga";
                }
                return RedirectToAction("Index","Home");
            }
            catch
            {
                return View();
            }
        }

        // GET: Training/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Training/Delete/5
        [HttpPost]
        public ActionResult Delete(FormCollection collection)
        {
            try
            {
                Training t = Training.GetTraining(Convert.ToInt32(collection["training_id"]));
                if (t.trainer_id != Convert.ToInt32(Session["uid"]))
                {
                    TempData["error"] = "Nemate kontrolu nad ovim treningom";
                }
                else if (t.delete() == 1)
                {
                    TempData["success"] = "Uspesno uklonjen trening";
                }
                else
                {
                    TempData["error"] = "Doslo je do greske prilikom uklanjanja treninga";
                }
                return RedirectToAction("Index", "User");
            }
            catch
            {
                return View();
            }
        }
        public ActionResult Trainings(int id)
        {
            List<Appointment> appointments = Appointment.GetAppointments();
            appointments.RemoveAll(app => app.training_id != id);
            List<Models.User> users = new List<Models.User>();
            foreach (var appointment in appointments)
            {
                users.Add(Models.User.GetUser(appointment.user_id));
            }
            ViewBag.users = users;
            return View();
        }
        public ActionResult Search()
        {
            return null;
        }

    }
}
