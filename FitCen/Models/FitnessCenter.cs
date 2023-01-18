using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace FitCen.Models
{
    public class FitnessCenter
    {
        [DisplayName("ID")]
        public int id { get; set; }

        [DisplayName("Naziv")]
        public string name { get; set; }

        [DisplayName("Adresa")]
        public string address { get; set; }

        [DisplayName("Godina otvaranja")]
        public int year_opened { get; set; }

        [DisplayName("ID vlasnika")]
        public int owner_id { get; set; }

        [DisplayName("Mesecna cena")]
        public int month_price { get; set; }

        [DisplayName("Godisnja cena")]
        public int year_price { get; set; }

        [DisplayName("Cena grupnog treninga")]
        public int group_price { get; set; }

        [DisplayName("Cena grupnog treninga s trenerom")]
        public int group_price_with_trainer { get; set; }

        public FitnessCenter()
        {

        }
        public FitnessCenter(string name, string address, int year_opened, int owner_id, int month_price, int year_price, int group_price, int group_price_with_trainer)
        {
            this.name = name;
            this.address = address;
            this.year_opened = year_opened;
            this.owner_id = owner_id;
            this.month_price = month_price;
            this.year_price = year_price;
            this.group_price = group_price;
            this.group_price_with_trainer = group_price_with_trainer;
        }
        public static int getID()
        {
            int cur = 0;
            foreach (var center in FitnessCenter.GetCenters())
            {
                if (center.id > cur)
                {
                    cur = center.id;
                }
            }
            return cur + 1;
        }
        public static FitnessCenter GetCenter(int id)
        {
            List<FitnessCenter> centers = FitnessCenter.GetCenters();
            foreach (var center in centers)
            {
                if (center.id == id)
                {
                    return center;
                }
            }
            return null;
        }
        public static List<FitnessCenter> GetCenters()
        {
            String data = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/App_Data/centers.json"));
            List<FitnessCenter> centers = JsonConvert.DeserializeObject<List<FitnessCenter>>(data);
            return centers;
        }
        public int create()
        {
            List<FitnessCenter> centers = FitnessCenter.GetCenters();
            centers.Add(this);
            string json = JsonConvert.SerializeObject(centers);
            System.IO.File.WriteAllText(HostingEnvironment.MapPath("~/App_Data/centers.json"), json);
            return 1;
        }
        public int update()
        {
            List<FitnessCenter> centers = FitnessCenter.GetCenters();
            foreach (var center in centers)
            {
                if (center.id == this.id)
                {
                    center.name = this.name;
                    center.address = this.address;
                    center.year_opened = this.year_opened;
                    center.group_price = this.group_price;
                    center.group_price_with_trainer = this.group_price_with_trainer;
                    center.month_price = this.month_price;
                    center.year_price = this.year_price;
                }
            }
            string json = JsonConvert.SerializeObject(centers);
            System.IO.File.WriteAllText(HostingEnvironment.MapPath("~/App_Data/centers.json"), json);
            return 1;
        }
        public int delete()
        {
            FitnessCenter center = FitnessCenter.GetCenter(this.id);
            List<dynamic> trainings = Training.GetTrainings(center.id);
            foreach (var training in trainings)
            {
                List<Appointment> appointments = Appointment.GetTrainingAppointments(training.id);
                if (appointments.Count != 0)
                {
                    return -1;
                }
            }
            foreach (var training in trainings)
            {
                training.delete();
            }
            List<User> users = User.GetCenterTrainers(center.id);
            foreach (var user in users)
            {
                user.block();
            }

            List<FitnessCenter> centers = FitnessCenter.GetCenters();
            foreach (var c in centers)
            {
                if (c.id == this.id)
                {
                    centers.Remove(c);
                    break;
                }
            }
            string json = JsonConvert.SerializeObject(centers);
            System.IO.File.WriteAllText(HostingEnvironment.MapPath("~/App_Data/centers.json"), json);
            return 1;
        }
    }
}