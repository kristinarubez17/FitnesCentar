using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using Newtonsoft.Json;

namespace FitCen.Models
{
    public class User
    {
        public int id { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public int gender { get; set; }
        public int role_id { get; set; }
        public int center_id { get; set; }
        public DateTime birthday { get; set; }
        public int owner_id { get; set; }

        public User()
        {
            this.role_id = 1;
            this.center_id = 0;
        }
        public User(string username, string email, string password, string firstname, string lastname, int gender, int role_id, int center_id, DateTime birthday)
        {
            this.username = username;
            this.email = email;
            this.password = password;
            this.firstname = firstname;
            this.lastname = lastname;
            this.gender = gender;
            this.role_id = role_id;
            this.center_id = center_id;
            this.birthday = birthday;
        }
        public static int getID()
        {
            int cur = 0;
            foreach (var user in User.GetUsers())
            {
                if (user.id > cur)
                {
                    cur = user.id;
                }
            }
            return cur + 1;
        }
        public static User GetUser(int id)
        {
            List<User> users = User.GetUsers();
            foreach (var user in users)
            {
                if (user.id == id)
                {
                    return user;
                }
            }
            return null;
        }
        public static User GetUser(string email, string password)
        {
            List<User> users = User.GetUsers();
            foreach (var user in users)
            {
                if (user.username.CompareTo(email) == 0 && user.password.CompareTo(password) == 0)
                {
                    return user;
                }
            }
            return null;
        }
        public static List<User> GetUsers()
        {
            String data = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/App_Data/users.json"));
            List<User> users = JsonConvert.DeserializeObject<List<User>>(data);
            return users;
        }
        public int create()
        {
            List<User> users = User.GetUsers();
            users.Add(this);
            string json = JsonConvert.SerializeObject(users);
            System.IO.File.WriteAllText(HostingEnvironment.MapPath("~/App_Data/users.json"), json);
            return 1;
        }
        public int update()
        {
            List<User> users = User.GetUsers();
            foreach (var user in users)
            {
                if (user.id == this.id)
                {
                    user.username = this.username;
                    user.email = this.email;
                    user.password = this.password;
                    user.firstname = this.firstname;
                    user.lastname = this.lastname;
                    user.gender = this.gender;
                    user.role_id = this.role_id;
                    user.center_id = this.center_id;
                    user.birthday = this.birthday;
                }
            }
            string json = JsonConvert.SerializeObject(users);
            System.IO.File.WriteAllText(HostingEnvironment.MapPath("~/App_Data/users.json"), json);
            return 1;
        }
        public int delete()
        {
            List<User> users = User.GetUsers();
            foreach (var u in users)
            {
                if (u.id == this.id)
                {
                    users.Remove(u);
                    break;
                }
            }
            string json = JsonConvert.SerializeObject(users);
            System.IO.File.WriteAllText(HostingEnvironment.MapPath("~/App_Data/users.json"), json);
            return 1;
        }

        internal static List<User> GetCenterTrainers(int id)
        {
            List<User> users = User.GetUsers();
            users.RemoveAll(user => user.center_id != id);
            return users;
        }

        public int block()
        {
            bool found = false;
            List<string> blockedUsers = System.IO.File.ReadAllLines(HostingEnvironment.MapPath("~/App_Data/blockedusers.csv")).ToList();
            foreach (var line in blockedUsers)
            {
                if (Convert.ToInt32(line) == this.id)
                {
                    found = true;
                }
            }
            if (found)
            {
                return -1;
            }
            else
            {
                blockedUsers.Add(this.id.ToString());
                System.IO.File.WriteAllLines(HostingEnvironment.MapPath("~/App_Data/blockedusers.csv"), blockedUsers.ToArray());

                return 1;
            }
        }
        public int unblock()
        {
            bool found = false;
            List<string> blockedUsers = System.IO.File.ReadAllLines(HostingEnvironment.MapPath("~/App_Data/blockedusers.csv")).ToList();
            foreach (var line in blockedUsers)
            {
                if (Convert.ToInt32(line) == this.id)
                {
                    blockedUsers.Remove(line);
                    break;
                }
            }
            System.IO.File.WriteAllLines(HostingEnvironment.MapPath("~/App_Data/blockedusers.csv"), blockedUsers.ToArray());
            return 1;
        }
        public bool isBlocked() {
            List<string> blockedUsers = System.IO.File.ReadAllLines(HostingEnvironment.MapPath("~/App_Data/blockedusers.csv")).ToList();
            foreach (var line in blockedUsers)
            {
                if (Convert.ToInt32(line) == this.id)
                {
                    return true;
                }
            }
            return false;
        }
    }
}