using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace FitCen.Models
{
    public class Review
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public int training_id { get; set; }
        public int review { get; set; }
        public string comment { get; set; }
        public int display { get; set; }

        public Review() { }
        public Review(int user_id, int training_id, int review, string comment)
        {
            this.user_id = user_id;
            this.training_id = training_id;
            this.review = review;
            this.comment = comment;
        }
        public static int GetID()
        {
            int cur = 0;
            foreach (var r in Review.GetReviews())
            {
                if (r.id > cur)
                {
                    cur = r.id;
                }
            }
            return cur + 1;
        }
        public static List<Review> GetReviews()
        {
            String data = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/App_Data/reviews.json"));
            List<Review> reviews = JsonConvert.DeserializeObject<List<Review>>(data);
            return reviews;
        }
        public static List<dynamic> GetReviews(int id)
        {
            List<Review> reviews = Review.GetReviews();
            reviews.RemoveAll(r => r.user_id != id);
            List<dynamic> dyn = new List<dynamic>();
            foreach (var r in reviews)
            {
                dynamic re = new ExpandoObject();
                re.id = r.id;
                re.user_id = r.user_id;
                re.training_id = r.training_id;
                re.review = r.review;
                re.comment = r.comment;
                re.display = r.display;
                re.training_name = Training.GetTraining(r.training_id).name;
                re.center_name = FitnessCenter.GetCenter(Training.GetTraining(r.training_id).center_id).name;
                dyn.Add(re);
            }
            return dyn;
        }
        public static List<dynamic> GetReviewsForOwner(int id)
        {
            List<Review> reviews = Review.GetReviews();
            List<Review> filtered = new List<Review>();
            foreach (var r in reviews)
            {
                Training t = Training.GetTraining(r.training_id);
                FitnessCenter c = FitnessCenter.GetCenter(t.center_id);
                if (c.owner_id == id)
                {
                    filtered.Add(r);
                }
            }
            List<dynamic> dyn = new List<dynamic>();
            foreach (var r in filtered)
            {
                dynamic re = new ExpandoObject();
                Models.User u = Models.User.GetUser(r.user_id);
                Training t = Training.GetTraining(r.training_id);
                FitnessCenter c = FitnessCenter.GetCenter(t.center_id);
                re.id = r.id;
                re.user_id = r.user_id;
                re.userfirstname = u.firstname;
                re.userlastname = u.lastname;
                re.email = u.email;
                re.username = u.username;
                re.training_id = r.training_id;
                re.trainingname = t.name;
                re.review = r.review;
                re.comment = r.comment;
                re.display = r.display;
                re.training_name = Training.GetTraining(r.training_id).name;
                re.center_name = FitnessCenter.GetCenter(Training.GetTraining(r.training_id).center_id).name;
                dyn.Add(re);
            }
            return dyn;
        }

        public static List<dynamic> GetReviewsForCenter(int center_id)
        {
            List<Review> allReviews = Review.GetReviews();
            List<Review> filtered = new List<Review>();
            List<dynamic> dyn = new List<dynamic>();
            foreach (var r in allReviews)
            {
                Training t = Training.GetTraining(r.training_id);
                if (t.center_id == center_id)
                {
                    filtered.Add(r);
                }
            }
            foreach (var r in filtered)
            {
                dynamic re = new ExpandoObject();
                re.id = r.id;
                re.user_id = r.user_id;
                re.training_id = r.training_id;
                re.review = r.review;
                re.comment = r.comment;
                re.display = r.display;
                re.training_name = Training.GetTraining(r.training_id).name;
                re.center_name = FitnessCenter.GetCenter(Training.GetTraining(r.training_id).center_id).name;
                re.username = User.GetUser(r.user_id);
                dyn.Add(re);
            }
            return dyn;
        }

        public static Review GetReview(int id)
        {
            return Review.GetReviews().Find(r => r.id == id);
        }

        public int create()
        {
            List<Review> reviews = Review.GetReviews();
            reviews.Add(this);
            string json = JsonConvert.SerializeObject(reviews);
            System.IO.File.WriteAllText(HostingEnvironment.MapPath("~/App_Data/reviews.json"), json);
            return 1;
        }
        public int update()
        {
            List<Review> reviews = Review.GetReviews();
            foreach (var t in reviews)
            {
                if (t.id == this.id)
                {
                    t.user_id = this.user_id;
                    t.training_id = this.training_id;
                    t.review = this.review;
                    t.comment = this.comment;
                    t.display = this.display;
                }
            }
            string json = JsonConvert.SerializeObject(reviews);
            System.IO.File.WriteAllText(HostingEnvironment.MapPath("~/App_Data/reviews.json"), json);
            return 1;
        }
        public int delete()
        {
            List<Review> reviews = Review.GetReviews();
            foreach (var t in reviews)
            {
                if (t.id == this.id)
                {
                    reviews.Remove(t);
                    break;
                }
            }
            string json = JsonConvert.SerializeObject(reviews);
            System.IO.File.WriteAllText(HostingEnvironment.MapPath("~/App_Data/reviews.json"), json);
            return 1;
        }
        public int enableReview()
        {
            this.display = 1;
            this.update();
            return 1;
        }
        public int disableReview()
        {
            this.display = 0;
            this.update();
            return 1;
        }


    }
}