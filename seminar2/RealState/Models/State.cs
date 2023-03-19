using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RealState.Models
{
    public class State
    {
        [Required]
        public string State_Id { get; set; }
        //[Required]
        public string user_Id { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public string BuyRent { get; set; }
        [Required]
        public string Offers { get; set; }
        [Required]
        public string link { get; set; }
        [Required]
        public float Price { get; set; }
        [Required]
        public int Bedrooms { get; set; }
        [Required]
        public int Bathrooms { get; set; }
        [Required]
        public float Area { get; set; }
        [Required]
        public String Furnished { get; set; }
        [Required]
        public String City { get; set; }
        [Required]
        public String Description { get; set; }
        [Required]
        public String Security { get; set; }
        [Required]
        public String Balcony { get; set; }
        [Required]
        public String Private_Garden { get; set; }
        [Required]
        public String Pets_Allowed { get; set; }
        [Required]
        public String Covered_Parking { get; set; }
        [Required]
        public String Maids_Room { get; set; }
        [Required]
        public String Electricity_Meter { get; set; }
        [Required]
        public String Natural_Gas { get; set; }
        [Required]
        public String Landline { get; set; }
        [Required]
        public String Pool { get; set; }
        [Required]
        public String Central_heating { get; set; }
        [Required]
        public String Built_in_Kitchen_Appliances { get; set; }
        [Required]
        public String Elevator { get; set; }
        [Required]
        public String PaymentOption { get; set; }
    }
}