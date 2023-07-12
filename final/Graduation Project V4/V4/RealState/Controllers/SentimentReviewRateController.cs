using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RealState.Models;
using RealState.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Drawing.Printing;
using System.Net.Http;
using System.Threading.Tasks;
using static System.Windows.Forms.AxHost;
using Firebase.Auth;
using State = RealState.Models.State;
using System.Web.WebPages;

namespace RealState.Controllers
{
    public class SentimentReviewRateController : Controller
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

        MySession session = MySession.Instance;

        public ActionResult My_User_Review(int? page)
        {

            //Get All Users2Ids
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Users2Ids");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var all_ReviewRate = new List<ReviewRate>();
            if (data != null)
            {
                foreach (var item in data)
                {
                    if (item != null) // add another null check here
                    {
                        all_ReviewRate.Add(JsonConvert.DeserializeObject<ReviewRate>(((JProperty)item).Value.ToString()));
                    }
                }
            }

            // get all Users2Ids that contain Buyer ID 
            var BuyerReviewRate = new List<ReviewRate>();
            foreach (var r in all_ReviewRate)
            {
                if (r.BuyerId == session.user_id)
                {
                    BuyerReviewRate.Add(r);
                }
            }

            //Get All Users
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response2 = client.Get("Users");
            dynamic data2 = JsonConvert.DeserializeObject<dynamic>(response2.Body);
            var all_Users = new List<SignUpModel>();
            foreach (var item in data2)
            {
                all_Users.Add(JsonConvert.DeserializeObject<SignUpModel>(((JProperty)item).Value.ToString()));
            }

            //Get Users that treated with Buyer
            var UsersWithBuyer = new List<SignUpModel>();
            foreach (var r in BuyerReviewRate)
            {
                foreach (var u in all_Users)
                {
                    if (r.SellerId == u.SignUp_ID)
                    {
                        UsersWithBuyer.Add(u);
                    }
                }       
            }    
            if (UsersWithBuyer.Count() == 0)
            {
                return RedirectToAction("ReviewNotFound");
            }
            else
            {
                var Users = UsersWithBuyer.ToPagedList(page ?? 1, UsersWithBuyer.Count());
                return View(Users);
            }
        }
        
        public async Task<ActionResult> ReviewSentiment(string Seller_id)
        {
            //get Review from view 
            string review = Request.Form["review"];
            string Sentiment_Category = "";
            //using Sentiment Model to know if text positive or negative
            // Check if input text is not empty
            if (!string.IsNullOrEmpty(review))
            {
                //string jsonResponse="";
                string apiUrl = "http://localhost:5000/";
                //string inputText = "This movie is really good!";
                using (HttpClient client = new HttpClient())
                {
                    var parameters = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("q", review)
                    });
                    HttpResponseMessage response = await client.PostAsync(apiUrl, parameters);
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        
                        var jsonOutput = response.Content.ReadAsStringAsync().Result;
                        dynamic result = JsonConvert.DeserializeObject(jsonOutput);
                        Sentiment_Category = (string)result.sentiment;                   
                    }
                    else
                    {
                        Console.WriteLine("Request failed with status code: " + response.StatusCode);
                    }
                }
            }   

            if (Seller_id == null)
            {
                return RedirectToAction("ReviewNotFound");
            }


            // add User History for users
            UserHistory userhistory = new UserHistory();
            userhistory.MyID = Seller_id;
            userhistory.Review = review;



            // get Seller to update his rate
            //Get All Users
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response0 = client.Get("Users");
            dynamic data0 = JsonConvert.DeserializeObject<dynamic>(response0.Body);
            var all_Users = new List<SignUpModel>();
            foreach (var item in data0)
            {
                all_Users.Add(JsonConvert.DeserializeObject<SignUpModel>(((JProperty)item).Value.ToString()));
            }

            foreach (var item in all_Users)
            {
                if (item.SignUp_ID == Seller_id)
                {
                    item.TotalReviews += 1;
                    if (Sentiment_Category.Equals("Positive"))
                    {
                        item.PositiveReviews += 1;
                    }
                    client.Set("Users/" + item.SignUp_ID, item);
                    break;
                }
            }


            //Get All Estates
            FirebaseResponse response4 = client.Get("States");
            dynamic data4 = JsonConvert.DeserializeObject<dynamic>(response4.Body);
            var all_Estates = new List<State>();
            foreach (var item in data4)
            {
                all_Estates.Add(JsonConvert.DeserializeObject<State>(((JProperty)item).Value.ToString()));
            }


            //now we need to remove the seller from My_review { review only one time }
            //Get All Users2Ids
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response3 = client.Get("Users2Ids");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response3.Body);
            var all_ReviewRate = new List<ReviewRate>();
            foreach (var item in data)
            {
                all_ReviewRate.Add(JsonConvert.DeserializeObject<ReviewRate>(((JProperty)item).Value.ToString()));
            }
            foreach (ReviewRate item in all_ReviewRate)
            {
                if (item.SellerId == Seller_id)
                {
                    // get State ID and Reviewr
                    userhistory.EstateId = item.EstateId;
                    userhistory.Reviewr = item.BuyerId;

                    // get Estate To Store its pic and price
                    foreach(State Estate in all_Estates)
                    {
                        if (Estate.State_Id == item.EstateId)
                        {
                            userhistory.EstatePic = Estate.link;
                            userhistory.EstatePrice = Estate.Price;
                        }
                    }

                    foreach (SignUpModel user in all_Users)
                    {
                        if (user.SignUp_ID == item.BuyerId)
                        {
                            // store reviewr name in user story to represent it in history
                            userhistory.ReviewrName = user.Name;
                        }
                    }

                    //store UserHistory in firebase
                    var auth = new FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig(Apikey));
                    var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, Authpassword);
                    AddUserHistoryToFirebase(userhistory);

                    // then remove the reviewRateID
                    client = new FireSharp.FirebaseClient(config);
                    FirebaseResponse response5 = client.Delete("Users2Ids/" + item.ReviewRateId);
                    
                }
            }
            


            return RedirectToAction("My_User_Review", "SentimentReviewRate");
        }
        //Function to add userhistory
        private void AddUserHistoryToFirebase(UserHistory data)
        {
            client = new FireSharp.FirebaseClient(config);

            PushResponse response = client.Push("UserHistory/", data);
            data.UserHistoryID = response.Result.name;
            client.Set("UserHistory/" + data.UserHistoryID, data);
        }
        //no user found
        public ActionResult ReviewNotFound()
        {
            return View();
        }

        public ActionResult ShowHistory(int? page,string id) 
        {
            //Get All User History
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response0 = client.Get("UserHistory");
            dynamic data0 = JsonConvert.DeserializeObject<dynamic>(response0.Body);
            var all_Histories = new List<UserHistory>();
            foreach (var item in data0)
            {
                all_Histories.Add(JsonConvert.DeserializeObject<UserHistory>(((JProperty)item).Value.ToString()));
            }

            string checkid = session.user_id;
            if (!id.IsEmpty())
            {
                checkid = id;
            }
            //Get UsersHisory for sign up user
            var UsersHistories = new List<UserHistory>();
            foreach (UserHistory item in all_Histories)
            {
                if (item.MyID == checkid)
                {
                    UsersHistories.Add(item);
                }
            }
            if (UsersHistories.Count() == 0)
            {
                return RedirectToAction("ReviewNotFound");
            }
            else
            {
                var Histories = UsersHistories.ToPagedList(page ?? 1, UsersHistories.Count());
                return View(Histories);
            }
        }
    }
}