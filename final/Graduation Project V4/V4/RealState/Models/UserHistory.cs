using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RealState.Models
{
    public class UserHistory
    {

        public string MyID { get; set; }
        public string Reviewr { get; set; }
        public string EstatePic { get; set; }
        public float EstatePrice { get; set; }
        public string Review { get; set; }
        public string EstateId { get; set; }
        public string ReviewrName { get; set; }
        public string UserHistoryID { get; set; }
    }
}