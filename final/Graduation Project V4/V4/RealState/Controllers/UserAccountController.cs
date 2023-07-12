using Firebase.Auth;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using RealState.Models;
using FireSharp.Interfaces;
using FireSharp.Response;
using RealState.Session;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading;
using Firebase.Storage;
using System.Text;

namespace RealState.Controllers
{
    public class UserAccountController : Controller
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

        // GET: Account
        public ActionResult SignUp()
        {
            var Questions = new List<string>();
            Questions.Add("Select Question...");
            Questions.Add("what's your Fav color?");
            Questions.Add("when last time you fail?");
            Questions.Add("where are you live?");
            ViewBag.Questions = new SelectList(Questions);
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> SignUp(SignUpModel model)
        {
            var Questions = new List<string>();
            Questions.Add("Select Question...");
            Questions.Add("what's your Fav color?");
            Questions.Add("when last time you fail?");
            Questions.Add("where are you live?");
            ViewBag.Questions = new SelectList(Questions);
            try
            {
                if (!model.Name.Equals(""))
                {
                    if (model.Question != "Select Question...")
                    {
                        if (model.Password == model.ConfirmPassword)
                        {
                            if (model.Password.Count() >= 6)
                            {
                                if ((model.Password.Contains("&") || model.Password.Contains("_") || model.Password.Contains("#")))
                                {

                                    var auth = new FirebaseAuthProvider(new FirebaseConfig(Apikey));
                                    var a = await auth.CreateUserWithEmailAndPasswordAsync(model.Email, model.Password, model.Name, true);

                                    model.My_interested = "";
                                    model.Bio = "";
                                    model.link = "";
                                    AddAccToFirebase(model);

                                    ModelState.AddModelError(string.Empty, "Please Verify your email then login.");
                                    TempData["AlertMessageSignup"] = "Please Verify your email then login.";
                                }
                                else
                                {
                                    ModelState.AddModelError(string.Empty, "password should contain at least one of (&,#,_)");
                                    TempData["AlertMessageSignup"] = "password should contain at least one of (&,#,_)";
                                }
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, "Your Password should be 6 digits or more");
                                TempData["AlertMessageSignup"] = "Your Password should be 6 digits or more";
                            }
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Confirmation password doesn't match!");
                            TempData["AlertMessageSignup"] = "Confirmation password doesn't match!";
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Please select Question!");
                        TempData["AlertMessageSignup"] = "Please select Question!";
                    }

                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Your information not right!");
                    TempData["AlertMessageSignup"] = "Your information not right!";
                }
            }
            catch (Exception ex)
            {
                //ModelState.AddModelError(string.Empty, ex.Message);
                ModelState.AddModelError(string.Empty, "Your information not right!");
                TempData["AlertMessageSignup"] = "Your information not right!";
            }

            return View();
        }

        private void AddAccToFirebase(SignUpModel model)
        {
            client = new FireSharp.FirebaseClient(config);

            model.SignUp_ID = model.Email;
            PushResponse response = client.Push("Users/", model);

            model.SignUp_ID = response.Result.name;
            SetResponse setResponse = client.Set("Users/" + model.SignUp_ID, model);
        }

        /* private void UpdateAccInFirebase(SignUpModel model)
         {
             client = new FireSharp.FirebaseClient(config);

             model.SignUp_ID = model.Email;
             UpdateRespons response = client.Update("Users/", model);

             model.SignUp_ID = response.Result.name;
             SetResponse setResponse = client.Set("Users/" + model.SignUp_ID, model);
         }*/

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            try
            {
                // Verification.
                if (this.Request.IsAuthenticated)
                {
                    //return this.RedirectToAction("States", "States");
                    //  return this.RedirectToLocal(returnUrl);
                }
            }
            catch (Exception ex)
            {
                // Info
                Console.Write(ex);
            }

            // Info.
            return this.View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginviewModel model, string returnUrl)
        {
            //get email from view 
            string Email = Request["Email"];
            string password = Request["password"];
            MySession session = MySession.Instance;


            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Users");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<SignUpModel>();
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<SignUpModel>(((JProperty)item).Value.ToString()));
            }
            foreach (var User in list)
            {
                if (User.Email == Email)
                {
                    //get user's Email => unique
                    session.user_id = User.SignUp_ID;
                    session.user_Email = User.Email;
                    session.User_Password = User.Password;
                    session.User_Name = User.Name;
                    session.User_Bio = User.Bio;
                    session.user_PositiveReviews = User.PositiveReviews;
                    session.user_TotalReviews = User.TotalReviews;
                    session.User_ConfPassword = User.ConfirmPassword;
                    session.User_Phone = User.PhoneNumber;
                    session.User_link = User.link;
                    session.Question = User.Question;
                    session.Answer = User.Answer;
                    session.login_times = User.login_times;
                    session.scoreSelled = User.scoreSelled;
                    session.My_interested = User.My_interested;

                }
            }

            try
            {
                // Verification.
                if (ModelState.IsValid)
                {
                    if (session.user_Email == "adnadn153153@gmail.com" && session.User_Password == "adnadn1531532020#")
                    {
                        return RedirectToAction("ShowContact", "Admin");
                    }
                    if (/*token != "" */session.user_Email == Email && session.User_Password == password)
                    {
                        //this.SignInUser(user.Email, token, false);
                        if (session.login_times == 0)
                        {
                            // update login times for next time
                            SignUpModel mod = new SignUpModel();
                            client = new FireSharp.FirebaseClient(config);
                            mod.SignUp_ID = session.user_id;
                            mod.Email = session.user_Email;
                            mod.PositiveReviews = session.user_PositiveReviews;
                            mod.TotalReviews = session.user_TotalReviews;
                            mod.link = session.User_link;
                            mod.Question = session.Question;
                            mod.Answer = session.Answer;
                            mod.Password = session.User_Password;
                            mod.ConfirmPassword = session.User_ConfPassword;
                            mod.login_times = session.login_times + 1;
                            mod.link = session.User_link;
                            mod.Bio = session.User_Bio;
                            mod.PhoneNumber = session.User_Phone;
                            mod.Name = session.User_Name;
                            mod.My_interested = session.My_interested;
                            mod.scoreSelled = session.scoreSelled;

                            SetResponse response2 = client.Set("Users/" + mod.SignUp_ID, mod);

                            // go to interested page
                            return RedirectToAction("interest", "UserAccount");
                        }
                        else
                        {
                            // update login times for next time
                            SignUpModel mod = new SignUpModel();
                            client = new FireSharp.FirebaseClient(config);
                            mod.SignUp_ID = session.user_id;
                            mod.Email = session.user_Email;
                            mod.PositiveReviews = session.user_PositiveReviews;
                            mod.TotalReviews = session.user_TotalReviews;
                            mod.link = session.User_link;
                            mod.Question = session.Question;
                            mod.Answer = session.Answer;
                            mod.Password = session.User_Password;
                            mod.ConfirmPassword = session.User_ConfPassword;
                            mod.login_times = session.login_times + 1;
                            mod.link = session.User_link;
                            mod.Bio = session.User_Bio;
                            mod.PhoneNumber = session.User_Phone;
                            mod.Name = session.User_Name;
                            mod.My_interested = session.My_interested;
                            mod.scoreSelled = session.scoreSelled;

                            SetResponse response2 = client.Set("Users/" + mod.SignUp_ID, mod);

                            //got to home page
                            return RedirectToAction("States", "States");
                        }

                    }
                    else
                    {
                        // Setting.
                        ModelState.AddModelError(string.Empty, "Invalid username or password.");
                        TempData["AlertMessageSignup"] = "Invalid username or password.";
                    }
                }
            }
            catch (Exception ex)
            {
                // Info

                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                TempData["AlertMessageSignup"] = "Invalid username or password.";
                //Console.Write(ex);
            }

            // If we got this far, something failed, redisplay form
            return this.View(model);
        }


        [HttpGet]
        public ActionResult interest()
        {
            List<InterestedModel> interested = new List<InterestedModel>();
            interested.Add(new InterestedModel() { Name = "Securiy", icon = "fa-solid fa-shield-halved", IsCheck = false });
            interested.Add(new InterestedModel() { Name = "Pooling", icon = "fa-solid fa-person-swimming", IsCheck = false });
            interested.Add(new InterestedModel() { Name = "Electricity", icon = "fa-solid fa-bolt", IsCheck = false });
            interested.Add(new InterestedModel() { Name = "Bathrooms", icon = "fa-solid fa-bath", IsCheck = false });
            interested.Add(new InterestedModel() { Name = "Price", icon = "fa-solid fa-sack-dollar", IsCheck = false });
            interested.Add(new InterestedModel() { Name = "Area", icon = "fa-solid fa-chart-area", IsCheck = false });
            interested.Add(new InterestedModel() { Name = "Pets Allowed", icon = "fa-solid fa-bath", IsCheck = false });
            interested.Add(new InterestedModel() { Name = "Elevator", icon = "fa-solid fa-elevator", IsCheck = false });
            interested.Add(new InterestedModel() { Name = "Gas", icon = "fa-solid fa-gas-pump", IsCheck = false });
            InterestedList intrlist = new InterestedList();
            intrlist.interests = interested;

            return View(intrlist);
        }
        [HttpPost]
        public ActionResult interest(InterestedList intrlist)
        {
            StringBuilder x = new StringBuilder();
            MySession session = MySession.Instance;

            List<string> list = new List<string>();

            foreach (var item in intrlist.interests)
            {
                if (item.IsCheck)
                {
                    x.Append(item.Name + ',');

                }
            }
            string ss = x.ToString();
            list = ss.Split(',').ToList();
            list.RemoveAt(list.Count - 1);
            session.My_interested = string.Join(" - ", list);

            // update login times for next time
            SignUpModel mod = new SignUpModel();
            client = new FireSharp.FirebaseClient(config);
            mod.SignUp_ID = session.user_id;
            mod.Email = session.user_Email;
            mod.PositiveReviews = session.user_PositiveReviews;
            mod.TotalReviews = session.user_TotalReviews;
            mod.link = session.User_link;
            mod.Question = session.Question;
            mod.Answer = session.Answer;
            mod.Password = session.User_Password;
            mod.ConfirmPassword = session.User_ConfPassword;
            mod.login_times = session.login_times + 1;
            mod.link = session.User_link;
            mod.Bio = session.User_Bio;
            mod.PhoneNumber = session.User_Phone;
            mod.Name = session.User_Name;
            mod.My_interested = session.My_interested;
            mod.scoreSelled = session.scoreSelled;

            SetResponse response = client.Set("Users/" + mod.SignUp_ID, mod);


            //@Html.CheckBoxFor(m => Model.interests[i].IsCheck)
            //got to home page
            return RedirectToAction("States", "States");
        }

        [HttpGet]
        public ActionResult ForgetPassword()
        {

            var Questions = new List<string>();
            Questions.Add("Select Question...");
            Questions.Add("what's your Fav color?");
            Questions.Add("when last time you fail?");
            Questions.Add("where are you live?");
            ViewBag.Questions = new SelectList(Questions);

            return View();
        }

        [HttpPost]
        public ActionResult ForgetPassword(SignUpModel log)
        {

            var Questions = new List<string>();
            Questions.Add("Select Question...");
            Questions.Add("what's your Fav color?");
            Questions.Add("when last time you fail?");
            Questions.Add("where are you live?");
            ViewBag.Questions = new SelectList(Questions);

            Boolean check = false;

            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Users");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<SignUpModel>();
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<SignUpModel>(((JProperty)item).Value.ToString()));
            }
            foreach (var User in list)
            {
                if (User.Email == log.Email && User.Question == log.Question && User.Answer == log.Answer)
                {
                    check = true;
                    MySession session = MySession.Instance;

                    //get user's Email => unique
                    session.user_id = User.SignUp_ID;
                    session.user_Email = User.Email;
                    session.User_Password = User.Password;
                    session.User_Name = User.Name;
                    session.User_Bio = User.Bio;
                    session.user_PositiveReviews = User.PositiveReviews;
                    session.user_TotalReviews = User.TotalReviews;
                    session.User_ConfPassword = User.ConfirmPassword;
                    session.User_Phone = User.PhoneNumber;
                    session.User_link = User.link;
                    session.Question = User.Question;
                    session.Answer = User.Answer;
                    session.login_times = User.login_times;
                    session.My_interested = User.My_interested;
                    session.scoreSelled = User.scoreSelled;
                }
            }

            try
            {
                if (check)
                {
                    return RedirectToAction("ChangePassword", "UserAccount");
                }
                else
                {
                    // Setting.
                    ModelState.AddModelError(string.Empty, "Make Sure your e-mail, question & answer are right.");
                    TempData["AlertMessageforgetpassword"] = "Make Sure your e-mail, question & answer are right.";
                }

            }
            catch (Exception ex)
            {
                // Setting.
                ModelState.AddModelError(string.Empty, "Make Sure your e-mail, question & answer are right.");
                TempData["AlertMessageforgetpassword"] = "Make Sure your e-mail, question & answer are right.";
            }


            return View();
        }


        [HttpGet]
        public ActionResult ChangePassword()
        {

            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(SignUpModel chang)
        {
            MySession session = MySession.Instance;
            SignUpModel mod = new SignUpModel();

            if (chang.Password == chang.ConfirmPassword)
            {
                mod.SignUp_ID = session.user_id;
                mod.Email = session.user_Email;
                mod.Password = chang.Password;
                mod.ConfirmPassword = chang.ConfirmPassword;
                mod.Bio = session.User_Bio;
                mod.PositiveReviews = session.user_PositiveReviews;
                mod.TotalReviews = session.user_TotalReviews;
                mod.Name = session.User_Name;
                mod.PhoneNumber = session.User_Phone;
                mod.link = session.User_link;
                mod.Question = session.Question;
                mod.Answer = session.Answer;
                mod.login_times = session.login_times;
                mod.My_interested = session.My_interested;
                mod.scoreSelled = session.scoreSelled;

                client = new FireSharp.FirebaseClient(config);

                SetResponse response = client.Set("Users/" + mod.SignUp_ID, mod);

                return RedirectToAction("Login", "UserAccount");

            }
            else
            {
                // Setting.
                ModelState.AddModelError(string.Empty, "confirm password doesn't match password");
                TempData["AlertMessagechangepassword"] = "confirm password doesn't match password";
            }
            return View();
        }


        [AllowAnonymous]
        [HttpGet]
        public ActionResult LogOff()
        {
            var ctx = Request.GetOwinContext();
            var authenticationManager = ctx.Authentication;
            authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            MySession session = MySession.Instance;
            session.user_id = "";
            session.user_Email = "";
            session.User_Password = "";
            session.User_Name = "";
            session.User_Bio = "";
            session.user_PositiveReviews = 0;
            session.user_TotalReviews = 0;
            session.User_ConfPassword = "";
            session.User_Phone = "";
            session.User_link = "";
            session.My_interested = "";
            session.login_times = 0;
            session.scoreSelled = 0;
            return RedirectToAction("Login", "UserAccount");
        }


        [customeAuthorized]
        [HttpGet]
        public ActionResult profile()
        {

            // return our user for displaying
            MySession session = MySession.Instance;
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Users/" + session.user_id);
            SignUpModel user = JsonConvert.DeserializeObject<SignUpModel>(response.Body);
            return View(user);
        }


        [customeAuthorized]
        public ActionResult profileWithUserId(string userId)
        {
            // return our user for displaying
            MySession session = MySession.Instance;
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Users/" + userId);

            SignUpModel user = JsonConvert.DeserializeObject<SignUpModel>(response.Body);
            ViewBag.sessionId = session.user_id;
            return View(user);
        }
        [customeAuthorized]
        [HttpPost]
        //profile
        public async Task<ActionResult> profile(HttpPostedFileBase file)
        {
            MySession session = MySession.Instance;
            // return our user for displaying
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Users/" + session.user_id);
            SignUpModel user = JsonConvert.DeserializeObject<SignUpModel>(response.Body);


            // image
            FileStream stream;
            string link = session.User_link;
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
            try
            {
                user.link = link;
                ModelState.AddModelError(string.Empty, "Added Successfully");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Added Successfully");
            }
            //model.link = link;

            session.User_link = link;

            SetResponse response0 = client.Set("Users/" + user.SignUp_ID, user);

            return RedirectToAction("profile", FormMethod.Get);
        }


        [customeAuthorized]
        [HttpGet]
        public ActionResult EditAccount()
        {

            MySession session = MySession.Instance;
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Users/" + session.user_id);

            SignUpModel user = JsonConvert.DeserializeObject<SignUpModel>(response.Body);

            return View(user);
        }

        [customeAuthorized]
        [HttpPost]
        public ActionResult EditAccount(SignUpModel model)
        {
            // get account
            MySession session = MySession.Instance;
            client = new FireSharp.FirebaseClient(config);
            model.Email = session.user_Email;
            model.PositiveReviews = session.user_PositiveReviews;
            model.TotalReviews = session.user_TotalReviews;
            model.link = session.User_link;
            model.Question = session.Question;
            model.Answer = session.Answer;
            model.Password = session.User_Password;
            model.ConfirmPassword = session.User_ConfPassword;
            model.login_times = session.login_times;
            model.My_interested = session.My_interested;
            model.scoreSelled = session.scoreSelled;
            session.User_Bio = model.Bio;
            session.User_Name = model.Name;
            session.User_Phone = model.PhoneNumber;
            SetResponse response = client.Set("Users/" + model.SignUp_ID, model);

            return RedirectToAction("profile");
        }



        // Need to add it in EStates Page
        [HttpGet]
        public ActionResult recommend_govrn()
        {
            Governorate go = new Governorate();
            List<string> Govren = go.Governorates_list();
            ViewBag.govern = new SelectList(Govren);

            return View(new govrnee());
        }
        [HttpPost]
        public ActionResult recommend_govrn(govrnee mm, FormCollection form)
        {
            Governorate go = new Governorate();
            List<string> Govren = new List<string>();
            Govren = go.Governorates_list();
            ViewBag.govern = new SelectList(Govren);


            string govern_name = mm.Question;

            List<string> nearst = new List<string>();
            if (govern_name != "None")
            {
                nearst = go.get_nearest_govn(govern_name);
            }
            else
            {
                nearst.Add(govern_name);
            }

            var model = new govrnee { Strings = nearst };


            //return RedirectToAction("States", "state");
            return View("recommend_govrn", model);
        }

    }
}