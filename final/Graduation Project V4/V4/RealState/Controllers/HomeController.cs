using Firebase.Auth;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PagedList;
using RealState.Models;
using RealState.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RealState.Controllers
{
    [customeAuthorized]
    public class HomeController : Controller
    {

        //Home
        //firebase
        private static string Apikey = "AIzaSyBmuTMtbd9-jCEH55r7eAvI1YC0NC-wLXY";
        private static string Bucket = "realestate-e6dce.appspot.com";
        private static string AuthEmail = "asramhmd130@gmail.com";
        private static string Authpassword = "Esraa#777";
        IFirebaseConfig config = new FireSharp.Config.FirebaseConfig
        {
            AuthSecret = "iadmfN20Fsw6gtWkpeA2ZICLuWDrRIqTDNGLUQLb",
            BasePath = "https://realestate-e6dce-default-rtdb.firebaseio.com/"
        };
        public IFirebaseConfig Config { get => config; set => config = value; }
        IFirebaseClient client;

        MySession session = MySession.Instance;
        public ActionResult Index(int? page)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse r = client.Get("Users");
            dynamic users = JsonConvert.DeserializeObject<dynamic>(r.Body);
            var usersList = new List<SignUpModel>();
            var listScore = new List<int>();
            if (users == null)
            {
                users = "";
            }
            foreach (var item in users)
            {
                usersList.Add(JsonConvert.DeserializeObject<SignUpModel>(((JProperty)item).Value.ToString()));
            }
            var order=usersList.OrderByDescending(u=>u.scoreSelled).ToList();
            var topTen=new List<SignUpModel>();
            for(var i = 0; i < order.Count(); i++)
            {
                if (i == 10)
                {
                    break;
                }
                topTen.Add(order[i]);
            }
            return View(topTen.ToPagedList(page ?? 1, 3));
        }


        public ActionResult States()
        {
            return View();
        }

        public ActionResult contactUs()
        {
            return View();
        }

        [HttpPost]
        public ActionResult contactUs(String msg)
        {
            //string subject = "Contact Us";
            //WebMail.Send("asramhmd130@gmail.com", subject, msg,null,null,null,true,null,null,null,null,null,null);
            //ViewBag.send = "Sent Successfully.....";

            //MailMessage m = new MailMessage(session.user_Email, "asramhmd130@gmail.com");
            //m.Subject = "contact Us";
            //m.Body = msg;
            //m.IsBodyHtml = false;
            //SmtpClient smtp = new SmtpClient();
            //smtp.Host = "smtp.gmail.com";
            //smtp.Port = 587;
            //smtp.EnableSsl = true;
            //NetworkCredential nc = new NetworkCredential("em4138811@gmail.com", "Esraamohamed777");
            //smtp.UseDefaultCredentials = false;
            //smtp.Credentials = nc;
            //smtp.Send(m);

            ContactUs contact = new ContactUs();
            contact.Msg = msg;
            contact.userId = session.user_id;
            contact.Name = session.User_Name;
            AddContactToFirebase(contact);

            return RedirectToAction("Index");
        }
        private void AddContactToFirebase(ContactUs data)
        {
            client = new FireSharp.FirebaseClient(config);
            PushResponse response = client.Push("ContactUs/", data);
            data.contactId = response.Result.name;
            SetResponse setResponse = client.Set("ContactUs/" + data.contactId, data);
        }

    }
}