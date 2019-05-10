using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DartUserRegistrationService.Models
{
    public class Gamer
    {
        [DisplayName("Firstname")]
        [Required]
        public string FirstName { get; set; }
        [DisplayName("Lastname")]
        [Required]
        public string LastName { get; set; }
        [DisplayName("Email")]
        [Required]
        [EmailAddress]
        public string Email { get; set; }


    }
}
