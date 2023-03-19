using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
namespace RealState.Models
{
    public class SignUpModel
    {
        [Key]
        public String SignUp_ID { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }


        public string link { get; set; }


        [Display(Name = "Bio")]
        public string Bio { get; set; }
        

        [Display(Name = "Rate")]
        public int Rate { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone")]
        public string PhoneNumber { get; set; }


    }
}