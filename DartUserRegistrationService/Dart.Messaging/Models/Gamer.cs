using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dart.Messaging.Models
{
    public class Gamer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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

        public GamerMinimal ToMinimal()
        {
            return new GamerMinimal
            {
                Id = Id,
                Name = DisplayName,
                MaxTries = 3
            };
        }

    }
    public class GamerMinimal
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int MaxTries { get; set; }
    }
}
