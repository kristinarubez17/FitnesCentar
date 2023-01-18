using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FitCen.Models;

namespace FitCen.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            ViewBag.search = null;
            ViewBag.time_lower = DateTime.MinValue;
            ViewBag.time_upper = DateTime.MaxValue;
            ViewBag.sorting = null;
            ViewBag.sorting_order = null;
            if (Session["rid"] != null)
            {
                if (Convert.ToInt32(Session["rid"]) == 1)
                {
                    List<dynamic> upcoming_trainings = Training.getUpcomingForUser(Convert.ToInt32(Session["uid"]));
                    if (upcoming_trainings == null)
                    {
                        upcoming_trainings = new List<dynamic>();
                    }
                    List<dynamic> past_trainings = Training.getPassedForUser(Convert.ToInt32(Session["uid"]));
                    if (past_trainings == null)
                    {
                        past_trainings = new List<dynamic>();
                    }
                    List<dynamic> ratings = Review.GetReviews(Convert.ToInt32(Session["uid"]));
                    ViewBag.upcoming_trainings = upcoming_trainings;
                    ViewBag.past_trainings = past_trainings;
                    ViewBag.ratings = ratings;
                    return View();
                }
                if (Convert.ToInt32(Session["rid"]) == 2)
                {

                    List<dynamic> upcoming_trainings = Training.getUpcomingForTrainer(Convert.ToInt32(Session["uid"]));
                    List<dynamic> past_trainings = Training.getPassedForTrainer(Convert.ToInt32(Session["uid"]));
                    if (upcoming_trainings == null)
                    {
                        upcoming_trainings = new List<dynamic>();
                    }
                    if (past_trainings == null)
                    {
                        past_trainings = new List<dynamic>();
                    }
                    ViewBag.upcoming_trainings = upcoming_trainings;
                    ViewBag.past_trainings = past_trainings;

                    return View("TrainerView");
                }
                if (Convert.ToInt32(Session["rid"]) == 3)
                {
                    List<FitCen.Models.User> users = FitCen.Models.User.GetUsers();
                    List<dynamic> parsed = new List<dynamic>();
                    List<FitnessCenter> centers = FitnessCenter.GetCenters();
                    List<dynamic> parsedCenters = new List<dynamic>();
                    List<dynamic> reviews = Review.GetReviewsForOwner(Convert.ToInt32(Session["uid"]));
                    int uid = Convert.ToInt32(Session["uid"]);
                    centers.RemoveAll(c => c.owner_id != uid);
                    users.RemoveAll(u => u.owner_id != Convert.ToInt32(Session["uid"]));
                    foreach (var user in users)
                    {
                        dynamic u = new ExpandoObject();
                        u.id = user.id;
                        u.username = user.username;
                        u.email = user.email;
                        u.firstname = user.firstname;
                        u.lastname = user.lastname;
                        u.password = user.password;
                        u.center_id = user.center_id;
                        u.center_name = (user.center_id != 0) ? FitnessCenter.GetCenter(user.center_id).name : "";
                        u.isBlocked = user.isBlocked();
                        if (user.gender == 1)
                        {
                            u.gender = "Muski";
                        }
                        else
                        {
                            u.gender = "Zenski";
                        }
                        u.birthday = user.birthday.ToShortDateString();
                        parsed.Add(u);
                    }
                    foreach (var center in centers)
                    {
                        dynamic c = new ExpandoObject();
                        c.id = center.id;
                        c.name = center.name;
                        c.address = center.address;
                        c.year_opened = center.year_opened;
                        c.month_price = center.month_price;
                        c.year_price = center.year_price;
                        c.group_price = center.group_price;
                        c.group_price_with_trainer = center.group_price_with_trainer;
                        parsedCenters.Add(c);
                    }
                    ViewBag.trainers = parsed;
                    ViewBag.centers = parsedCenters;
                    ViewBag.ratings = reviews;
                    return View("OwnerView");

                }
            }
            return RedirectToAction("Index", "Home");
        }
        public ActionResult Search()
        {
            if (Session["rid"] != null)
            {
                if (Convert.ToInt32(Session["rid"]) == 1)
                {
                    List<dynamic> upcoming_trainings = Training.getUpcomingForUser(Convert.ToInt32(Session["uid"]));
                    if (upcoming_trainings == null)
                    {
                        upcoming_trainings = new List<dynamic>();
                    }
                    List<dynamic> past_trainings = Training.getPassedForUser(Convert.ToInt32(Session["uid"]));
                    if (past_trainings == null)
                    {
                        past_trainings = new List<dynamic>();
                    }
                    List<dynamic> ratings = Review.GetReviews(Convert.ToInt32(Session["uid"]));

                    string searchText = Request.QueryString["search"];
                    DateTime time_upper = (Request.QueryString["time_upper"] != "") ? Convert.ToDateTime(Request.QueryString["time_upper"]) : DateTime.MaxValue;
                    DateTime time_lower = (Request.QueryString["time_lower"] != "") ? Convert.ToDateTime(Request.QueryString["time_lower"]) : DateTime.MinValue;
                    string sorting = Request.QueryString["sorting"];
                    string sorting_order = Request.QueryString["sorting_order"];
                    List<dynamic> tr = new List<dynamic>();

                    foreach (var t in past_trainings)
                    {
                        if (t.name.Contains(searchText) || t.training_type.Contains(searchText) || t.center_name.Contains(searchText))
                        {
                            tr.Add(t);
                        }
                    }
                    List<dynamic> sp = new List<dynamic>();
                    foreach (var t in tr)
                    {
                        if (DateTime.Compare(t.start_full, time_lower) <= 0 && DateTime.Compare(t.start_full, time_upper) >= 0)
                        {
                            sp.Add(t);
                        }
                    }
                    if (sp.Count != 0)
                    {
                        if (sorting != "Sortiranje")
                        {
                            if (sorting == "name")
                            {
                                sp.OrderBy(t => t.name);
                            }
                            else if (sorting == "type")
                            {
                                sp.OrderBy(t => t.training_type);
                            }
                            else if (sorting == "time")
                            {
                                sp.OrderBy(t => t.start);
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
                        ViewBag.past_trainings = sp;
                    }
                    else
                    {
                        if (time_upper == DateTime.MaxValue && time_lower == DateTime.MinValue)
                        {
                            ViewBag.past_trainings = past_trainings;
                        }
                        else
                        {
                            ViewBag.past_trainings = new List<dynamic>();
                        }
                    }
                    ViewBag.search = searchText;
                    ViewBag.time_lower = (time_lower == DateTime.MinValue) ? DateTime.MinValue : time_lower;
                    ViewBag.time_upper = (time_upper == DateTime.MaxValue) ? DateTime.MaxValue : time_upper;
                    ViewBag.sorting = sorting;
                    ViewBag.sorting_order = sorting_order;

                    ViewBag.upcoming_trainings = upcoming_trainings;
                    //ViewBag.past_trainings = tr;
                    ViewBag.ratings = ratings;
                    return View("Index");
                }
                else if (Convert.ToInt32(Session["rid"]) == 2)
                {
                    string searchText = Request.QueryString["search"];
                    string type = Request.QueryString["type"];
                    DateTime time_upper = (Request.QueryString["time_upper"] != "") ? Convert.ToDateTime(Request.QueryString["time_upper"]) : DateTime.MaxValue;
                    DateTime time_lower = (Request.QueryString["time_lower"] != "") ? Convert.ToDateTime(Request.QueryString["time_lower"]) : DateTime.MinValue;
                    string sorting = Request.QueryString["sorting"];
                    string sorting_order = Request.QueryString["sorting_order"];

                    List<dynamic> past_trainings = Training.getPassedForTrainer(Convert.ToInt32(Session["uid"]));

                    string searchType = Request.QueryString["multiparam"];
                    List<dynamic> spp = new List<dynamic>();
                    List<dynamic> filtered_past = new List<dynamic>();

                    if (searchType == "1")
                    {
                        foreach (var u in past_trainings)
                        {
                            if (u.name.Contains(searchText) || u.training_type.Contains(type))
                            {
                                filtered_past.Add(u);
                            }
                        }
                    }
                    else if (searchType == "2")
                    {
                        foreach (var u in past_trainings)
                        {
                            if (u.name.Contains(searchText) && u.training_type.Contains(type))
                            {
                                filtered_past.Add(u);
                            }
                        }
                    }

                    foreach (var t in filtered_past)
                    {
                        if (DateTime.Compare(t.start, time_lower) >= 0 && DateTime.Compare(t.start, time_upper) <= 0)
                        {
                            spp.Add(t);
                        }
                    }
                    if (spp.Count != 0)
                    {
                        if (sorting != "Sortiranje")
                        {
                            if (sorting == "name")
                            {
                                spp.OrderBy(t => t.name);
                            }
                            else if (sorting == "type")
                            {
                                spp.OrderBy(t => t.training_type);
                            }
                            else if (sorting == "time")
                            {
                                spp.OrderBy(t => t.start);
                            }
                        }
                        if (sorting_order != "Poredak")
                        {
                            if (sorting_order == "ascending")
                            {

                            }
                            else if (sorting_order == "descending")
                            {
                                spp.Reverse();
                            }
                        }
                        ViewBag.past_trainings = spp;
                    }
                    List<dynamic> upcoming_trainings = Training.getUpcomingForUser(Convert.ToInt32(Session["uid"]));
                    ViewBag.upcoming_trainings = upcoming_trainings;
                    ViewBag.past_trainings = spp;
                    ViewBag.search = searchText;
                    ViewBag.searchType = type;
                    ViewBag.time_upper = time_upper;
                    ViewBag.time_lower = time_lower;
                    return View("TrainerView");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return Json("here 2 ", JsonRequestBehavior.AllowGet);

                return RedirectToAction("Index", "Home");
            }
        }

        // GET: User/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: User/Create
        public ActionResult Create(User userModel)
        {
            return View();
        }

        // POST: User/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                string username = collection["username"];
                string firstname = collection["firstname"];
                string lastname = collection["lastname"];
                string email = collection["email"];
                string password = collection["password"];
                int gender = Convert.ToInt32(collection["gender"]);
                DateTime birthday = Convert.ToDateTime(collection["birthday"]);
                User user = new User(username, email, password, firstname, lastname, gender, 0, 0, birthday);
                user.id = FitCen.Models.User.getID();
                user.create();
                Session["username"] = user.username;
                Session["uid"] = user.id;
                Session["rid"] = user.role_id;
                Session["email"] = user.email;
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: User/Edit/5
        public ActionResult Edit(int id)
        {
            int uid = -1;
            if (Session["uid"] != null)
            {
                uid = Convert.ToInt32(Session["uid"]);
                int rid = Convert.ToInt32(Session["rid"]);
                if (uid != id && rid != 3)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    User u = FitCen.Models.User.GetUser(id);
                    ViewBag.user = u;
                }
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: User/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                User u = FitCen.Models.User.GetUser(id);
                u.username = collection["username"];
                u.firstname = collection["firstname"];
                u.lastname = collection["lastname"];
                u.email = collection["email"];
                u.password = collection["password"];
                u.gender = Convert.ToInt32(collection["gender"]);
                u.birthday = Convert.ToDateTime(collection["birthday"]);
                if (collection["role_id"] != null)
                {
                    u.role_id = Convert.ToInt32(collection["role_id"]);
                }
                if (u.update() == 1)
                {
                    Session["username"] = u.username;
                    TempData["success"] = "Uspesno izmenjen korisnik";
                }
                else
                {
                    TempData["error"] = "Doslo je do greske prilikom izmene korisnika";
                }
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["error"] = "Doslo je do greske prilikom izmene korisnika";
                return View();
            }
        }

        // GET: User/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: User/Delete/5
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
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            List<FitCen.Models.User> users = FitCen.Models.User.GetUsers();
            foreach (var user in users)
            {
                if (user.username.CompareTo(collection["username"]) == 0 && user.password.CompareTo(collection["password"]) == 0)
                {
                    if (!user.isBlocked())
                    {
                        Session["username"] = user.username;
                        Session["uid"] = user.id;
                        Session["rid"] = user.role_id;
                        Session["email"] = user.email;
                        TempData["success"] = "Uspesno prijavljivanje. Dobrodosli!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["error"] = "Blokirani ste. Ne mozete se ulogovati.";
                        return RedirectToAction("Login", "User");

                    }
                }
            }
            TempData["error"] = "Pogresni podaci";
            return RedirectToAction("Login", "User");
        }
        public ActionResult Logout()
        {
            Session.RemoveAll();
            return RedirectToAction("Index", "Home");
        }
        public ActionResult Trainer()
        {
            return View("OwnerView");
        }
        [HttpPost]
        public ActionResult Trainer(FormCollection collection)
        {
            if (Session["rid"] != null && Convert.ToInt32(Session["rid"]) == 3)
            {
                string username = collection["username"];
                string password = collection["password"];
                string email = collection["email"];
                string firstname = collection["firstname"];
                string lastname = collection["lastname"];
                int gender = Convert.ToInt32(collection["gender"]);
                DateTime birthday = Convert.ToDateTime(collection["birthday"]);
                int id = FitCen.Models.User.getID();
                User trainer = new FitCen.Models.User(username, email, password, firstname, lastname, gender, 2, 0, birthday);
                trainer.id = id;
                trainer.owner_id = Convert.ToInt32(Session["uid"]);
                if (trainer.create() == 1)
                {
                    TempData["success"] = "Uspesno kreiran trener";
                }
                else
                {
                    TempData["error"] = "Doslo je do greske prilikom kreiranja treninga";
                }

            }
            else
            {
                TempData["error"] = "Morate biti vlasnik za pristup ovom delu aplikacije";
            }
            return RedirectToAction("Index", "User");
        }
        public ActionResult RemoveTrainer(int id)
        {
            FitCen.Models.User user = Models.User.GetUser(id);
            if (user.delete() == 1)
            {
                TempData["success"] = "Uspesno uklonjen korisnik";
            }
            else
            {
                TempData["error"] = "Doslo je do greske pri uklanjanju korisnika";
            }
            return RedirectToAction("Index", "User");
        }
        public ActionResult Block(int id)
        {
            Models.User user = Models.User.GetUser(id);
            if (user.block() == 1)
            {
                TempData["success"] = "Uspesno blokiran trener";
            }
            else
            {
                TempData["error"] = "Doslo je do greske prilikom blokiranja trenera";
            }
            return RedirectToAction("Index", "User");
        }
        public ActionResult Unblock(int id)
        {
            Models.User user = Models.User.GetUser(id);
            if (user.unblock() == 1)
            {
                TempData["success"] = "Uspesno odblokiran trener";
            }
            else
            {
                TempData["error"] = "Doslo je do greske prilikom odblokiranja trenera";
            }
            return RedirectToAction("Index", "User");

        }
        public ActionResult GiveCenter(FormCollection collection)
        {
            int user_id = Convert.ToInt32(collection["user_id"]);
            int center_id = Convert.ToInt32(collection["center_id"]);
            Models.User user = Models.User.GetUser(user_id);
            user.center_id = center_id;
            if (user.update() == 1)
            {
                TempData["success"] = "Uspesno dodljen fitnes centar treneru";
            }
            else
            {
                TempData["error"] = "Doslo je do greske prilikom dodele fitnes centra treneru";
            }
            return RedirectToAction("Index", "User");

        }
    }
}
