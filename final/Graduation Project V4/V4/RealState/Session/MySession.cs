using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RealState.Session
{
    public sealed class MySession
    {
        public string user_id { get; set; }
        public string user_Email { get; set; }
        public int user_PositiveReviews { get; set; }
        public int user_TotalReviews { get; set; }
        public int login_times { get; set; }
        public string User_Password { get; set; }
        public string User_ConfPassword { get; set; }
        public string User_Phone { get; set; }
        public string User_Name { get; set; }
        public string User_Bio { get; set; }
        public string User_link { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public string My_interested { get; set; }
        public int scoreSelled { get; set; }
        public string stateid { get; set; }


        private MySession() { }
        private static readonly object lock_obj = new object();
        private static MySession instance = null;
        public static MySession Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lock_obj)
                    {
                        if (instance == null)
                        {
                            instance = new MySession();
                        }
                    }
                }
                return instance;
            }
        }

        public int Property
        {
            get => default;
            set
            {
            }
        }
    }
}