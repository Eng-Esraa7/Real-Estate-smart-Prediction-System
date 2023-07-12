using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RealState.Models
{
    public class ContactUs
    {
        [Required]
        public String contactId { get; set; }
        [Required]
        public String Name { get; set; }
        [Required]
        public String userId { get; set; }
        [Required]
        public String Msg { get; set; }
    }
}