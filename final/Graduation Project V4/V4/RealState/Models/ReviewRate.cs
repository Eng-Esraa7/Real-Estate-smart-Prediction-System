using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RealState.Models
{
    public class ReviewRate
    {
        public string SellerId { get; set; }
        public string BuyerId { get; set; }
        public string ReviewRateId { get; set; }
        public string EstateId { get; set; }
    }
}