using Firebase.Auth;
using Firebase.Storage;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PagedList;
using RealState.Models;
using RealState.Session;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace RealState.Controllers
{
    [customeAuthorized]
    public class StatesController : Controller
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

        // GET: States
        public ActionResult States(string searchBy, String Searching, int? page)
        {

            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse r = client.Get("Fav");
            dynamic fav = JsonConvert.DeserializeObject<dynamic>(r.Body);
            var listFav = new List<Favorite>();
            if (fav == null)
            {
                fav = "";
            }
            foreach (var item in fav)
            {
                listFav.Add(JsonConvert.DeserializeObject<Favorite>(((JProperty)item).Value.ToString()));
            }
            //get fav

            ViewBag.fav = listFav;
            ViewBag.id = session.user_id;
            FirebaseResponse response = client.Get("States");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var l = new List<State>();
            if (data == null)
            {
                return RedirectToAction("StateNotFound");
            }
            foreach (var item in data)
            {
                l.Add(JsonConvert.DeserializeObject<State>(((JProperty)item).Value.ToString()));
            }
            var list = l.Where(x => x.Selled.ToString().Contains("no"));

            var states = list.ToPagedList(page ?? 1, 3);
            if (searchBy == "Price")
            {
                var filter = list.Where(x => x.Price.ToString().Contains(Searching));
                if (filter.Count() == 0)
                {
                    return RedirectToAction("StateNotFound");
                }
                return View(filter.ToList().ToPagedList(page ?? 1, filter.Count()));
            }
            else if (searchBy == "Area")
            {
                var filter = list.Where(x => x.Area.ToString().Contains(Searching));
                if (filter.Count() == 0)
                {
                    return RedirectToAction("StateNotFound");
                }
                return View(filter.ToList().ToPagedList(page ?? 1, filter.Count()));
            }
            else if (searchBy == "City")
            {
                var filter = list.Where(x => x.City.ToString().Contains(Searching));
                if (filter.Count() == 0)
                {
                    return RedirectToAction("StateNotFound");
                }
                return View(filter.ToList().ToPagedList(page ?? 1, filter.Count()));
            }
            else if (searchBy == "buy")
            {
                var filter = list.Where(x => x.BuyRent.ToString().Contains(Searching));
                if (filter.Count() == 0)
                {
                    return RedirectToAction("StateNotFound");
                }
                return View(filter.ToList().ToPagedList(page ?? 1, filter.Count()));
            }
            else if (searchBy == "rent")
            {
                var filter = list.Where(x => x.BuyRent.ToString().Contains(Searching));
                if (filter.Count() == 0)
                {
                    return RedirectToAction("StateNotFound");
                }
                return View(filter.ToList().ToPagedList(page ?? 1, filter.Count()));
            }
            else if (searchBy == "offers")
            {
                var filter = list.Where(x => x.Offers.ToString().Contains(Searching));
                if (filter.Count() == 0)
                {
                    return RedirectToAction("StateNotFound");
                }
                return View(filter.ToList().ToPagedList(page ?? 1, filter.Count()));
            }
            else
            {
                return View(states);
            }

        }

        //not found
        public ActionResult StateNotFound()
        {
            return View();
        }
        //get requests for specific state
        List<RequestToOwner> requestsForState(string stateId)
        {  
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response2 = client.Get("Requests/");

            FirebaseResponse r = client.Get("Requests");
            dynamic requests = JsonConvert.DeserializeObject<dynamic>(r.Body);
            var listRequesrs = new List<RequestToOwner>();
            var listRequesrsFilter = new List<RequestToOwner>();
            if (requests == null)
            {
                requests = "";
            }
            foreach (var item in requests)
            {
                listRequesrs.Add(JsonConvert.DeserializeObject<RequestToOwner>(((JProperty)item).Value.ToString()));
            }

            foreach (var req in listRequesrs)
            {
                if (req.StateId == stateId)
                    listRequesrsFilter.Add(req);
            }

            return listRequesrsFilter;
        }

        List<Favorite> FavForState(string stateId)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse r = client.Get("Fav");
            dynamic favs = JsonConvert.DeserializeObject<dynamic>(r.Body);
            var listfavss = new List<Favorite>();
            var listFilter = new List<Favorite>();
            if (favs == null)
            {
                favs = "";
            }
            foreach (var item in favs)
            {
                listfavss.Add(JsonConvert.DeserializeObject<Favorite>(((JProperty)item).Value.ToString()));
            }

            if (listfavss != null)
            {
                foreach (var f in listfavss)
                {
                    if (f.stateId == stateId)
                        listfavss.Add(f);
                }
            }


            return listfavss;
        }

        //details
        [HttpGet]
        public ActionResult Details(String id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("States/" + id);

            State state = JsonConvert.DeserializeObject<State>(response.Body);

            //get requests for specific state
            List<RequestToOwner> listRequesrsFilter = requestsForState(id);

            ViewModel view = new ViewModel();
            view.state = state;
            view.requests = listRequesrsFilter;
            ViewBag.id = session.user_id;
            return View(view);
        }

        public ActionResult CreateState()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> CreateState(State state, HttpPostedFileBase file)
        {
            FileStream stream;
            string link = null;
            //if submit img
            if (file.ContentLength > 0)
            {
                Random rnd = new Random();
                int num = rnd.Next();
                string path = Path.Combine(Server.MapPath("~/Content/images/"), file.FileName + num);
                file.SaveAs(path);
                stream = new FileStream(Path.Combine(path), FileMode.Open);

                var auth = new FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig(Apikey));
                var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, Authpassword);

                // you can use CancellationTokenSource to cancel the upload midway
                var cancellation = new CancellationTokenSource();

                var task = new FirebaseStorage(
                    Bucket,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true // when you cancel the upload, exception is thrown. By default no exception is thrown
                    })
                    .Child("images")
                    .Child(file.FileName)
                    .PutAsync(stream, cancellation.Token);
                try
                {
                    // error during upload will be thrown when you await the task
                    link = await task;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception was thrown: {0}", ex);
                }



            }
            MySession session = MySession.Instance;
            
            /// making the input to sent to the flaskApi
            var input = new List<Dictionary<string, object>>()
            {
                new Dictionary<string, object>{
                {"Payment_Option", state.PaymentOption},
                {"Area", state.Area},
                {"Bedrooms" ,state.Bedrooms},
                {"Bathrooms" ,state.Bathrooms},
                {"Furnished" ,state.Furnished},
                {"Security" ,state.Security},
                {"Balcony" ,state.Balcony},
                {"Private_Garden" ,state.Private_Garden},
                {"Pets_Allowed" ,state.Pets_Allowed},
                {"Covered_Parking" ,state.Covered_Parking},
                {"Maids_Room" ,state.Maids_Room},
                {"Electricity_Meter" ,state.Electricity_Meter},
                {"Landline" ,state.Landline},
                {"Natural_Gas" ,state.Natural_Gas},
                {"Pool" ,state.Pool},
                {"Central_heating" ,state.Central_heating},
                {"Built_in_Kitchen_Appliances" ,state.Built_in_Kitchen_Appliances},
                {"Elevator" ,state.Elevator}
                }
            };
            var jsonInput = JsonConvert.SerializeObject(input);
            var content = new StringContent(jsonInput, Encoding.UTF8, "application/json");

            var client = new HttpClient();
            var response = client.PostAsync("http://localhost:12345/predict", content).Result;
            var prediction = string.Empty;
            if (response.IsSuccessStatusCode)
            {
                var jsonOutput = response.Content.ReadAsStringAsync().Result;
                dynamic result = JsonConvert.DeserializeObject(jsonOutput);
                prediction = (string)result.prediction;
                Console.WriteLine("Prediction: " + prediction);
            }
            else
            {
                Console.WriteLine("API request failed");
            }
            
            ///end of api code
            ////////////////////////////////////////////////////////////////////////////////////////////
            try
            {
                // new code 
                state.user_name = session.User_Name;
                state.user_pic = session.User_link;
                //
                state.link = link;
                state.user_Id = session.user_id.ToString();
                state.Selled = "no";
                if (state.BuyRent == "Sell")
                {
                    state.Rentalperiod = "";
                }
                if (state.Offers == "No")
                {
                    state.priceOffer = 0;
                }
                AddStateToFirebase(state);
                ModelState.AddModelError(string.Empty, "Added Successfully");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            session.stateid = state.State_Id;
            return RedirectToAction("ShowPrediction", "States", new { predicted = prediction, actual = state.Price });
            //return RedirectToAction("States");


        }
        public ActionResult ShowPrediction(string predicted, string actual)
        {

            ViewBag.Predicted = predicted;
            ViewBag.Actual = actual;

            return View();
        }

        [HttpPost]
        public ActionResult ShowPrediction(string predict)
        {
            string state_id = session.stateid;
            session.stateid = string.Empty;
            if (!string.IsNullOrEmpty(predict))
            {
                //the predict button hava been clicked
                return RedirectToAction("Edit", "States", new { id = state_id });
            }
            else
            {
                //the actual button have been clicked
                return RedirectToAction("States");
            }



        }

        private void AddStateToFirebase(State data)
        {
            client = new FireSharp.FirebaseClient(config);

            PushResponse response = client.Push("States/", data);
            data.State_Id = response.Result.name;
            client.Set("States/" + data.State_Id, data);
        }

        [HttpGet]
        public ActionResult Edit(String id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("States/" + id);

            State state = JsonConvert.DeserializeObject<State>(response.Body);
            return View(state);
        }


        [HttpPost]
        public async Task<ActionResult> Edit(State state, HttpPostedFileBase file)
        {
            if (file != null)
            {
                FileStream stream;
                string link = null;
                //if submit img
                Random rnd = new Random();
                int num = rnd.Next();
                string path = Path.Combine(Server.MapPath("~/Content/images/"), file.FileName + num);
                file.SaveAs(path);
                stream = new FileStream(Path.Combine(path), FileMode.Open);

                var auth = new FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig(Apikey));
                var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, Authpassword);

                // you can use CancellationTokenSource to cancel the upload midway
                var cancellation = new CancellationTokenSource();

                var task = new FirebaseStorage(
                    Bucket,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true // when you cancel the upload, exception is thrown. By default no exception is thrown
                    })
                    .Child("images")
                    .Child(file.FileName)
                    .PutAsync(stream, cancellation.Token);
                try
                {
                    // error during upload will be thrown when you await the task
                    link = await task;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception was thrown: {0}", ex);
                }


                ////////////////////////////////////////////////////////////////////////////////////////////
                try
                {
                    state.link = link;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            if (state.BuyRent == "Sell")
            {
                state.Rentalperiod = "";
            }
            if (state.Offers == "No")
            {
                state.priceOffer = 0;
            }
            client = new FireSharp.FirebaseClient(config);
            state.Selled = "no";
            SetResponse response = client.Set("States/" + state.State_Id, state);
            //FirebaseResponse response2 = client.Delete("States/" + state.State_Id);
            return RedirectToAction("States");
        }

        [HttpGet]
        public ActionResult Delete(String id)
        {
            client = new FireSharp.FirebaseClient(config);
            //get req for state
            List<RequestToOwner> Requests = requestsForState(id);
            List<Favorite> favs = FavForState(id);
            //deleta all requests
            foreach (var r in Requests)
            {
                client.Delete("Requests/" + r.RequestId);
            }
            foreach (var f in favs)
            {
                client.Delete("Requests/" + f.favId);
            }
            //delete state
            FirebaseResponse response = client.Delete("States/" + id);
            return RedirectToAction("States");

        }

        //fav
        public ActionResult AddFav(String StateId)
        {
            Favorite fav = new Favorite();
            fav.userId = session.user_id;
            fav.stateId = StateId;
            AddFavToFirebase(fav);
            return RedirectToAction("States");
        }

        public ActionResult DeleteFav(String favId)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Delete("Fav/" + favId);
            return RedirectToAction("States");
        }

        private void AddFavToFirebase(Favorite data)
        {
            client = new FireSharp.FirebaseClient(config);
            PushResponse response = client.Push("Fav/", data);
            data.favId = response.Result.name;
            SetResponse setResponse = client.Set("Fav/" + data.favId, data);
        }

        public ActionResult Favorite()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse r = client.Get("Fav");
            dynamic fav = JsonConvert.DeserializeObject<dynamic>(r.Body);
            var listFav = new List<Favorite>();
            if (fav == null)
            {
                return RedirectToAction("StateNotFound");
            }
            foreach (var item in fav)
            {
                listFav.Add(JsonConvert.DeserializeObject<Favorite>(((JProperty)item).Value.ToString()));
            }

            FirebaseResponse response = client.Get("States");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<State>();
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<State>(((JProperty)item).Value.ToString()));
            }
            var filter = listFav.Where(x => x.userId.ToString().Contains(session.user_id));

            var state = new List<State>();
            foreach (var s in list)
            {
                foreach (var f in filter)
                {
                    if (s.State_Id == f.stateId)
                    {
                        state.Add(s);
                        break;
                    }
                }
            }
            return View(state);
        }

        public ActionResult SendEmail(string stateId)
        {
            //Get State
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("States/" + stateId);
            State state = JsonConvert.DeserializeObject<State>(response.Body);

            //Get email of owner state
            string userId = state.user_Id;
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response2 = client.Get("Users/" + userId);

            User user = JsonConvert.DeserializeObject<User>(response2.Body);

            string recipient = user.Email;
            string subject = "This Is Available!";
            string type;
            if (state.BuyRent == "Sell")
                type = "Buy";
            else
                type = state.BuyRent;
            string body = "I Want to " + type + " estate " + state.Description;
            string mailtoUrl = string.Format("mailto:{0}?subject={1}&body={2}", recipient, subject, body);

            //make request stateId requestUserId
            try
            {
                RequestToOwner request = new RequestToOwner();
                request.StateId = state.State_Id;
                request.requestUserId = session.user_id;
                request.NameUserRequest = session.User_Name;
                //Add to firebase
                client = new FireSharp.FirebaseClient(config);
                PushResponse respons = client.Push("Requests/", request);
                request.RequestId = respons.Result.name;
                SetResponse setResponse = client.Set("Requests/" + request.RequestId, request);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return Redirect(mailtoUrl);
        }

        public ActionResult Requests()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse r = client.Get("Requests");
            dynamic requests = JsonConvert.DeserializeObject<dynamic>(r.Body);
            var listRequesrs = new List<RequestToOwner>();
            var States = new List<State>();
            var listrequestfilter = new List<RequestToOwner>();
            if (requests == null)
            {
                return RedirectToAction("StateNotFound");
            }
            foreach (var item in requests)
            {
                listRequesrs.Add(JsonConvert.DeserializeObject<RequestToOwner>(((JProperty)item).Value.ToString()));
            }

            foreach (var req in listRequesrs)
            {
                if (req.requestUserId == session.user_id)
                {
                    //Get State
                    client = new FireSharp.FirebaseClient(config);
                    FirebaseResponse response = client.Get("States/" + req.StateId);
                    State state = JsonConvert.DeserializeObject<State>(response.Body);
                    States.Add(state);
                    listrequestfilter.Add(req);
                }
            }
            ViewModel view = new ViewModel();
            view.requests = listrequestfilter;
            view.states = States;
            return View(view);
        }

        [HttpGet]
        public ActionResult RemoveRequest(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Delete("Requests/" + id);
            return RedirectToAction("Requests");
        }

        public ActionResult ManageMyEstates(string selled, int? page)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("States/");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<State>();
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<State>(((JProperty)item).Value.ToString()));
            }
            var States = new List<State>();

            foreach (var s in list)
            {
                if (s.user_Id == session.user_id)
                {
                    States.Add(s);
                }
            }
            ViewBag.id = session.user_id;
            ViewBag.selled = selled;
            var filter = States.Where(x => x.Selled.ToString().Contains(selled));
            return View(filter);
        }

        public ActionResult DoneSell(string stateId)
        {
            //Get State
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("States/" + stateId);
            State state = JsonConvert.DeserializeObject<State>(response.Body);

            //get requests
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response2 = client.Get("Requests/");

            FirebaseResponse r = client.Get("Requests");
            dynamic requests = JsonConvert.DeserializeObject<dynamic>(r.Body);
            var listRequesrs = new List<RequestToOwner>();
            var listRequesrsFilter = new List<RequestToOwner>();
            foreach (var item in requests)
            {
                listRequesrs.Add(JsonConvert.DeserializeObject<RequestToOwner>(((JProperty)item).Value.ToString()));
            }

            foreach (var req in listRequesrs)
            {
                if (req.StateId == state.State_Id)
                    listRequesrsFilter.Add(req);
            }


            ViewModel view = new ViewModel();
            view.state = state;
            view.requests = listRequesrsFilter;
            return View(view);
        }
        public async Task<ActionResult> VerifySell(string stateId, string RequestuserId)
        {
            //make request stateId requestUserId
            try
            {
                EstateSelled selled = new EstateSelled();
                selled.StateId = stateId;
                selled.requestUserId = RequestuserId;
                selled.dateTime = DateTime.Now;

                //Add to firebase
                client = new FireSharp.FirebaseClient(config);
                PushResponse respons = client.Push("Selled/", selled);
                selled.SellDoneId = respons.Result.name;
                SetResponse setResponse = client.Set("Selled/" + selled.SellDoneId, selled);

                //update score
                FirebaseResponse response2 = client.Get("Users/" + session.user_id);
                SignUpModel user = JsonConvert.DeserializeObject<SignUpModel>(response2.Body);
                user.scoreSelled = user.scoreSelled + 1;
                client.Set("Users/" + user.SignUp_ID, user);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            //delete all requests
            //get req for state
            List<RequestToOwner> Requests = requestsForState(stateId);
            List<Favorite> favs = FavForState(stateId);
            //deleta all requests
            foreach (var r in Requests)
            {
                client.Delete("Requests/" + r.RequestId);
            }
            foreach (var f in favs)
            {
                client.Delete("Requests/" + f.favId);
            }
            //update states selled
            //Get State
            FirebaseResponse response = client.Get("States/" + stateId);
            State state = JsonConvert.DeserializeObject<State>(response.Body);
            state.Selled = "yes";
            client.Set("States/" + state.State_Id, state);


            // get Seller Id & Buyer Id For Review And Rating 
            string SellerId = session.user_id;
            string BuyerrId = RequestuserId;

            // create object from model ReviewRate
            ReviewRate reviewRate = new ReviewRate();
            reviewRate.SellerId = SellerId;
            reviewRate.BuyerId = BuyerrId;
            reviewRate.EstateId = stateId;

            //store reviewRate in firebase
            var auth = new FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig(Apikey));
            var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, Authpassword);
            AddReviewRateToFirebase(reviewRate);

            return RedirectToAction("ManageMyEstates", new { selled = "no" });
        }

        //Function to add users ids for review
        private void AddReviewRateToFirebase(ReviewRate data)
        {
            client = new FireSharp.FirebaseClient(config);

            PushResponse response = client.Push("Users2Ids/", data);
            data.ReviewRateId = response.Result.name;
            client.Set("Users2Ids/" + data.ReviewRateId, data);
        }

        public ActionResult RecommendationEstates(int? page)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("States");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var l = new List<State>();
            if (data == null)
            {
                return RedirectToAction("StateNotFound");
            }
            foreach (var item in data)
            {
                l.Add(JsonConvert.DeserializeObject<State>(((JProperty)item).Value.ToString()));
            }
            var list = l.Where(x => x.Selled.ToString().Contains("no"));

            var states = list.ToPagedList(page ?? 1, 3);
            string interested = session.My_interested;
            var filter=new List<State>();
            if (interested.Contains("Securiy"))
            {
                filter = list.Where(x => x.Security.ToString().Equals("Available")).ToList();
            }
            else
            {
                filter = list.ToList();
            }
            if (interested.Contains("Pooling"))
            {
                filter = filter.Where(x => x.Pool.ToString().Equals("Available")).ToList();
            }
            if (interested.Contains("Electricity"))
            {
                filter = filter.Where(x => x.Electricity_Meter.ToString().Equals("Available")).ToList();
            }
            //if (interested.Contains("Bathrooms"))
            //{
            //    filter = filter.Where(x => x.Bathrooms.ToString().Contains("Available")).ToList();
            //}
            //if (interested.Contains("Price"))
            //{
            //    filter = filter.Where(x => x.Price.ToString().Contains("Available")).ToList();
            //}
            //if (interested.Contains("Area"))
            //{
            //    filter = filter.Where(x => x.Area.ToString().Contains("Available")).ToList();
            //}
            if (interested.Contains("Pets Allowed"))
            {
                filter = filter.Where(x => x.Pets_Allowed.ToString().Equals("Available")).ToList();
            }
            if (interested.Contains("Elevator"))
            {
                filter = filter.Where(x => x.Elevator.ToString().Equals("Available")).ToList();
            }
            if (interested.Contains("Gas"))
            {
                filter = filter.Where(x => x.Natural_Gas.ToString().Equals("Available")).ToList();
            }

            return View(filter.ToList().ToPagedList(page ?? 1, 3));

        }
    }
}