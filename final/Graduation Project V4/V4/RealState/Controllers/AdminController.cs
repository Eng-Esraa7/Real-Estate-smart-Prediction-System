using FireSharp.Response;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RealState.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FireSharp.Interfaces;

namespace RealState.Controllers
{
    public class AdminController : Controller
    {
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
        public List<ContactUs> allcontactMsg()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse r = client.Get("ContactUs");
            dynamic c = JsonConvert.DeserializeObject<dynamic>(r.Body);
            var list = new List<ContactUs>();
            if (c == null)
            {
                c = "";
            }
            foreach (var item in c)
            {
                list.Add(JsonConvert.DeserializeObject<ContactUs>(((JProperty)item).Value.ToString()));
            }
            return list;
        }
        public ActionResult ShowContact()
        {
            var list = allcontactMsg();
            return View(list);
        }
        [HttpGet]
        public ActionResult Delete(String id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Delete("ContactUs/" + id);
            return RedirectToAction("ShowContact");

        }
    }
}