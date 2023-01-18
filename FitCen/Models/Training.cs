using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace FitCen.Models
{
    public class Training
    {
        [DisplayName("ID")]
        public int id { get; set; }

        [DisplayName("Naziv")]
        public string name { get; set; }

        [DisplayName("Tip treninga")]
        public string training_type { get; set; }

        [DisplayName("ID centra")]
        public int center_id { get; set; }

        [DisplayName("ID trenera")]
        public int trainer_id { get; set; }

        [DisplayName("Trajanje")]
        public int duration { get; set; }

        [DisplayName("Pocetak")]
        public DateTime start { get; set; }

        [DisplayName("Maksimalni kapacitet")]
        public int max_people { get; set; }
        public Training() { }
        public Training(string name, string training_type, int duration, DateTime start, int max_people)
        {
            this.name = name;
            this.training_type = training_type;
            this.duration = duration;
            this.training_type = training_type;
            this.start = start;
            this.max_people = max_people;
        }
        public static int getID()
        {
            int cur = 0;
            foreach (var training in Training.GetTrainings())
            {
                if (training.id > cur)
                {
                    cur = training.id;
                }
            }
            return cur + 1;
        }
        public static Training GetTraining(int id)
        {
            List<Training> trainings = Training.GetTrainings();
            foreach (var t in trainings)
            {
                if (t.id == id)
                {
                    return t;
                }
            }
            return null;
        }
        public static List<Training> GetTrainings()
        {
            String data = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/App_Data/trainings.json"));
            List<Training> trainings = JsonConvert.DeserializeObject<List<Training>>(data);
            return trainings;
        }
        public static List<dynamic> GetTrainings(int center_id)
        {
            List<Training> trainings = Training.GetTrainings();
            trainings.RemoveAll(tr => tr.center_id != center_id);
            List<dynamic> dyn = new List<dynamic>();
            foreach (var tr in trainings)
            {
                dynamic d = new ExpandoObject();

                d.id = tr.id;
                d.name = tr.name;
                d.training_type = tr.training_type;
                d.center_id = tr.center_id;
                d.trainer_id = tr.trainer_id;
                d.duration = tr.duration;
                d.start = tr.start;
                d.max_people = tr.max_people;
                d.booked = Appointment.GetBookedAppointments(tr.center_id).Count;
                dyn.Add(d);
            }
            return dyn;
        }
        public int create()
        {
            List<Training> trainings = Training.GetTrainings();
            trainings.Add(this);
            string json = JsonConvert.SerializeObject(trainings);
            System.IO.File.WriteAllText(HostingEnvironment.MapPath("~/App_Data/trainings.json"), json);
            return 1;
        }
        public int update()
        {
            List<Training> trainings = Training.GetTrainings();
            foreach (var t in trainings)
            {
                if (t.id == this.id)
                {
                    t.name = this.name;
                    t.start = this.start;
                    t.duration = this.duration;
                    t.max_people = this.max_people;
                }
            }
            string json = JsonConvert.SerializeObject(trainings);
            System.IO.File.WriteAllText(HostingEnvironment.MapPath("~/App_Data/trainings.json"), json);
            return 1;
        }
        public int delete()
        {
            List<Training> trainings = Training.GetTrainings();
            foreach (var t in trainings)
            {
                if (t.id == this.id)
                {
                    List<Appointment> appointments = Appointment.GetTrainingAppointments(t.id);
                    if (appointments.Count != 0)
                    {
                        return -1;
                    }
                    else
                    {
                        if (DateTime.Compare(t.start, DateTime.Now) < 0)
                        {
                            return -1;
                        }
                        else
                        {
                            trainings.Remove(t);
                            break;
                        }
                    }
                }
            }
            string json = JsonConvert.SerializeObject(trainings);
            System.IO.File.WriteAllText(HostingEnvironment.MapPath("~/App_Data/trainings.json"), json);
            return 1;
        }
        public static List<dynamic> getUpcomingForTrainer(int user_id)
        {
            List<Training> trainings = Training.GetTrainings();
            trainings.RemoveAll(t => DateTime.Compare(t.start, DateTime.Now) <= 0);
            trainings.RemoveAll(t => t.trainer_id != user_id);
            List<dynamic> dyn = new List<dynamic>();
            foreach (var t in trainings)
            {
                dynamic d = new ExpandoObject();
                d.id = t.id;
                d.name = t.name;
                d.training_type = t.training_type;
                d.center_id = t.center_id;
                d.trainer_id = t.trainer_id;
                d.duration = t.duration;
                d.start = t.start;
                d.max_people = t.max_people;
                d.free_places = t.max_people - Appointment.GetTrainingAppointments(t.id).Count;
                dyn.Add(d);
            }
            return dyn;
        }
        public static List<dynamic> getPassedForTrainer(int user_id)
        {
            List<Training> trainings = Training.GetTrainings();
            trainings.RemoveAll(t => DateTime.Compare(t.start, DateTime.Now) > 0);
            trainings.RemoveAll(t => t.trainer_id != user_id);
            List<dynamic> dyn = new List<dynamic>();
            foreach (var t in trainings)
            {
                dynamic d = new ExpandoObject();
                d.id = t.id;
                d.name = t.name;
                d.training_type = t.training_type;
                d.center_id = t.center_id;
                d.trainer_id = t.trainer_id;
                d.duration = t.duration;
                d.start = t.start;
                d.max_people = t.max_people;
                d.free_places = t.max_people - Appointment.GetTrainingAppointments(t.id).Count;
                dyn.Add(d);
            }
            return dyn;
        }
        public static List<dynamic> getForIndex()
        {
            List<Training> trainings = Training.GetTrainings();
            List<dynamic> dynamics = new List<dynamic>();
            trainings.RemoveAll(t => DateTime.Compare(t.start, DateTime.Now) <= 0);
            foreach (var t in trainings)
            {
                dynamic trainingExpanded = new ExpandoObject();
                trainingExpanded.id = t.id;
                trainingExpanded.name = t.name;
                trainingExpanded.training_type = t.training_type;
                trainingExpanded.center_id = t.center_id;
                trainingExpanded.center_name = FitnessCenter.GetCenter(t.center_id).name;
                trainingExpanded.center_address = FitnessCenter.GetCenter(t.center_id).address;
                trainingExpanded.center_opened = FitnessCenter.GetCenter(t.center_id).year_opened;
                trainingExpanded.trainer_name = FitCen.Models.User.GetUser(t.trainer_id);
                trainingExpanded.max_people = t.max_people;
                trainingExpanded.start = t.start.ToShortDateString();
                trainingExpanded.duration = t.duration;
                trainingExpanded.free_places = t.max_people - Appointment.GetTrainingAppointments(t.id).Count;
                dynamics.Add(trainingExpanded);
            }
            return dynamics;
        }
        public static List<dynamic> getUpcomingForUser(int id)
        {
            List<Appointment> apps = Appointment.GetAppointments();
            List<dynamic> dynamics = new List<dynamic>();
            foreach (var t in apps)
            {
                dynamic trainingExpanded = new ExpandoObject();
                Training tr = Training.GetTraining(t.training_id);
                if (DateTime.Compare(tr.start, DateTime.Now) > 0)
                {
                    trainingExpanded.id = t.id;
                    trainingExpanded.name = tr.name;
                    trainingExpanded.training_type = tr.training_type;
                    trainingExpanded.center_name = FitnessCenter.GetCenter(tr.center_id).name;
                    trainingExpanded.center_address = FitnessCenter.GetCenter(tr.center_id).address;
                    trainingExpanded.trainer_name = FitCen.Models.User.GetUser(t.user_id);
                    trainingExpanded.max_people = tr.max_people;
                    trainingExpanded.start = tr.start.ToShortDateString();
                    trainingExpanded.duration = tr.duration;
                    trainingExpanded.free_places = tr.max_people - Appointment.GetTrainingAppointments(tr.id).Count;
                    dynamics.Add(trainingExpanded);
                }
            }
            return dynamics;
        }
        public static List<dynamic> getPassedForUser(int id)
        {
            List<Appointment> apps = Appointment.GetAppointments(id);
            List<dynamic> dynamics = new List<dynamic>();
            foreach (var t in apps)
            {
                dynamic trainingExpanded = new ExpandoObject();
                Training tr = Training.GetTraining(t.training_id);
                if (DateTime.Compare(tr.start, DateTime.Now) <= 0)
                {
                    trainingExpanded.id = t.id;
                    trainingExpanded.name = tr.name;
                    trainingExpanded.training_type = tr.training_type;
                    trainingExpanded.center_name = FitnessCenter.GetCenter(tr.center_id).name;
                    trainingExpanded.center_address = FitnessCenter.GetCenter(tr.center_id).address;
                    trainingExpanded.trainer_name = FitCen.Models.User.GetUser(t.user_id);
                    trainingExpanded.max_people = tr.max_people;
                    trainingExpanded.start = tr.start.ToShortDateString();
                    trainingExpanded.start_full = tr.start;
                    trainingExpanded.duration = tr.duration;
                    trainingExpanded.free_places = tr.max_people - Appointment.GetTrainingAppointments(tr.id).Count;
                    dynamics.Add(trainingExpanded);
                }
            }
            return dynamics;
        }
        public static FitnessCenter GetFitnessCenter(int id)
        {
            Training training = Training.GetTraining(id);
            return FitnessCenter.GetCenter(training.center_id);
        }
    }
}