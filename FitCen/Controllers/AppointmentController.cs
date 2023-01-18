using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using FitCen.Models;

namespace FitCen.Controllers
{
    public class AppointmentController : Controller
    {
        // GET: Appointment
        public ActionResult Index()
        {
            return View();
        }

        // GET: Appointment/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Appointment/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Appointment/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                int user_id = Convert.ToInt32(Session["uid"]);
                if (user_id == 0)
                {
                    TempData["error"] = "Morate biti ulogovani za pristup do ovog dela aplikacije.";
                    return RedirectToAction("Index", "Home");

                }
                int training_id = Convert.ToInt32(collection["training_id"]);
                Appointment appointment = new Appointment(training_id, user_id);
                List<Appointment> appointments = Appointment.GetAppointments();
                appointment.id = Appointment.getID();
                foreach (var app in appointments)
                {
                    if (app.user_id == user_id && app.training_id == training_id)
                    {
                        TempData["error"] = "Vec postoji rezervacija na ovaj trening";
                        return RedirectToAction("Index", "Home");
                    }
                }
                if (appointment.create() == 1)
                {
                    TempData["success"] = "Uspesno zakazan termin";
                }
                else
                {
                    TempData["error"] = "Doslo je do greske prilikom zakazivanja termina";
                }
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                return View();
            }
        }

        // GET: Appointment/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Appointment/Edit/5
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

        // GET: Appointment/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Appointment/Delete/5
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
    }
}
