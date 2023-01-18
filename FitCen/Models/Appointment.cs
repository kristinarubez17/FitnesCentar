using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace FitCen.Models
{
    public class Appointment
    {
        public int id { get; set; }
        public int training_id { get; set; }
        public int user_id { get; set; }

        public Appointment() { }
        public Appointment(int training_id, int user_id)
        {
            this.training_id = training_id;
            this.user_id = user_id;
        }
        public static int getID()
        {
            int cur = 0;
            foreach (var training in Appointment.GetAppointments())
            {
                if (training.id > cur)
                {
                    cur = training.id;
                }
            }
            return cur + 1;
        }
        public static Appointment GetAppointment(int id)
        {
            List<Appointment> appointments = Appointment.GetAppointments();
            foreach (var t in appointments)
            {
                if (t.id == id)
                {
                    return t;
                }
            }
            return null;
        }
        public static List<Appointment> GetAppointments()
        {
            String data = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/App_Data/appointments.json"));
            List<Appointment> appointments = JsonConvert.DeserializeObject<List<Appointment>>(data);
            return appointments;
        }
        public static List<Appointment> GetBookedAppointments(int center_id){
            List<Appointment> appointments = Appointment.GetAppointments();
            List<Appointment> booked = new List<Appointment>();
            foreach (var t in appointments)
            {
                Training rt = Training.GetTraining(t.training_id);
                if (rt.center_id == center_id)
                {
                    booked.Add(t);
                }
            }
            return booked;
        }
        public static List<Appointment> GetAppointments(int id)
        {
            List<Appointment> apps = Appointment.GetAppointments();
            apps.RemoveAll(app => app.user_id != id);
            return apps;
        }
        public static List<Appointment> GetTrainingAppointments(int training_id)
        {
            List<Appointment> apps = Appointment.GetAppointments();
            apps.RemoveAll(app => app.training_id != training_id);
            return apps;
        }
        public int create()
        {
            List<Appointment> appointments = Appointment.GetAppointments();
            appointments.Add(this);
            string json = JsonConvert.SerializeObject(appointments);
            System.IO.File.WriteAllText(HostingEnvironment.MapPath("~/App_Data/appointments.json"), json);
            return 1;
        }
        public int update()
        {
            List<Appointment> appointments = Appointment.GetAppointments();
            foreach (var t in appointments)
            {
                if (t.id == this.id)
                {

                }
            }
            string json = JsonConvert.SerializeObject(appointments);
            System.IO.File.WriteAllText(HostingEnvironment.MapPath("~/App_Data/appointments.json"), json);
            return 1;
        }
        public int delete()
        {
            List<Appointment> appointments = Appointment.GetAppointments();
            foreach (var t in appointments)
            {
                if (t.id == this.id)
                {
                    appointments.Remove(t);
                    break;
                }
            }
            string json = JsonConvert.SerializeObject(appointments);
            System.IO.File.WriteAllText(HostingEnvironment.MapPath("~/App_Data/appointments.json"), json);
            return 1;
        }

    }
}