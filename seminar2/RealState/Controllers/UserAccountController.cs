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

namespace RealState.Controllers
{
    public class UserAccountController : Controller
    {
        //firebase
        private static string Apikey = "AIzaSyBmuTMtbd9-jCEH55r7eAvI1YC0NC-wLXY";
        private static string Bucket = "realestate-e6dce.appspot.com";
        private static string AuthEmail = "asramhmd130@gmail.com";
        private static string Authpassword = "esraa777";

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
            return View();
        } 
        

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> SignUp(SignUpModel model)
        {
            try
            {
                if (!model.Name.Equals(""))
                {
                    if (model.Password == model.ConfirmPassword)
                    {
                        if (model.Password.Count() >= 6)
                        {
                            if ((model.Password.Contains("&") || model.Password.Contains("_") || model.Password.Contains("#")))
                            {

                                var auth = new FirebaseAuthProvider(new FirebaseConfig(Apikey));
                                var a = await auth.CreateUserWithEmailAndPasswordAsync(model.Email, model.Password, model.Name, true);

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
        public async Task<ActionResult> Login(LoginviewModel model, string returnUrl )
        {

            try
            {
                // Verification.
                if (ModelState.IsValid)
                {
                    var auth = new FirebaseAuthProvider(new FirebaseConfig(Apikey));
                    var ab = await auth.SignInWithEmailAndPasswordAsync(model.Email, model.Password);
                    string token = ab.FirebaseToken;
                    var user = ab.User;
                    if (token != "")
                    {

                        this.SignInUser(user.Email, token, false);

                        MySession session = MySession.Instance;
                        //get email from view 
                        string Email = Request["Email"];


                        client = new FireSharp.FirebaseClient(config);
                        FirebaseResponse response = client.Get("Users");
                        dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
                        var list = new List<SignUpModel>();
                        foreach (var item in data)
                        {
                            list.Add(JsonConvert.DeserializeObject<SignUpModel>(((JProperty)item).Value.ToString()));
                        }
                        foreach (var item in list)
                        {
                            if (item.Email == Email)
                            {

                                //get user's Email => unique
                                session.user_id = item.SignUp_ID;
                                session.user_Email = item.Email;
                                session.User_Password = item.Password;
                                session.User_Name = item.Name;
                                session.User_Bio = item.Bio;
                                session.user_Rate = item.Rate;
                                session.User_ConfPassword = item.ConfirmPassword;
                                session.User_Phone = item.PhoneNumber;
                                session.User_link = item.link;
                            }
                        }


                        return RedirectToAction("States", "States");
                        //return this.RedirectToLocal(returnUrl);

                    }
                    else
                    {
                        // Setting.
                        ModelState.AddModelError(string.Empty, "Invalid username or password.");
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

        private void SignInUser(string email, string token, bool isPersistent)
        {
            // Initialization.
            var claims = new List<Claim>();

            try
            {
                // Setting
                claims.Add(new Claim(ClaimTypes.Email, email));
                claims.Add(new Claim(ClaimTypes.Authentication, token));
                var claimIdenties = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                var ctx = Request.GetOwinContext();
                var authenticationManager = ctx.Authentication;
                // Sign In.
                authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, claimIdenties);

                
            }
            catch (Exception ex)
            {
                // Info
                throw ex;
            }
        }

        private void ClaimIdentities(string username, bool isPersistent)
        {
            // Initialization.
            var claims = new List<Claim>();
            try
            {
                // Setting
                claims.Add(new Claim(ClaimTypes.Name, username));
                var claimIdenties = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

            }
            catch (Exception ex)
            {
                // Info
                throw ex;
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            try
            {
                // Verification.
                if (Url.IsLocalUrl(returnUrl))
                {
                    // Info.
                    return this.Redirect(returnUrl);
                }
            }
            catch (Exception ex)
            {
                // Info
                throw ex;
            }

            // Info.
            return this.RedirectToAction("LogOff", "UserAccount");
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult LogOff()
        {
            var ctx = Request.GetOwinContext();
            var authenticationManager = ctx.Authentication;
            authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "UserAccount");
        }



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


        [HttpGet]
        public ActionResult EditAccount()
        {

            MySession session = MySession.Instance;
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Users/" + session.user_id);

            SignUpModel user = JsonConvert.DeserializeObject<SignUpModel>(response.Body);

            return View(user);
        }


        [HttpPost]
        public ActionResult EditAccount(SignUpModel model)
        {
            // get account
            MySession session = MySession.Instance;
            client = new FireSharp.FirebaseClient(config);
            model.Email = session.user_Email;
            model.Rate = session.user_Rate;
            model.link = session.User_link;

            SetResponse response = client.Set("Users/" + model.SignUp_ID,model);
           
            return RedirectToAction("profile");
        }


    }
}