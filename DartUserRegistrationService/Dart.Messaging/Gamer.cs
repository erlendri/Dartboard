using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Dart.Messaging.Models
{
    public class Gamer
    {
        public Guid Id { get; set; }
        [DisplayName("Display name")]
        [Required]
        public string DisplayName { get; set; }
        [DisplayName("First name")]
        public string FirstName { get; set; }
        [DisplayName("Last name")]
        public string LastName { get; set; }
        [DisplayName("Email")]
        [EmailAddress]
        public string Email { get; set; }

        [DisplayName("QrCode")]
        public string QrCode { get; set; }

    }
}
