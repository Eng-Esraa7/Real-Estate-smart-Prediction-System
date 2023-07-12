using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RealState.Models
{
    public class EstateSelled
    {
        public string SellDoneId { get; set; }
        public string StateId { get; set; }
        public string requestUserId { get; set; }
        public DateTime dateTime { get; set; }
    }
}