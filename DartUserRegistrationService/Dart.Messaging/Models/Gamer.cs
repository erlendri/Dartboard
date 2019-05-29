﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

        public GamerMinimal ToMinimal()
        {
            return new GamerMinimal
            {
                Id = Id,
                Name = DisplayName,
                MaxTries = 3
            };
        }
        public List<Game> Games { get; set; } = new List<Game>();
    }
    public class GamerMinimal
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int MaxTries { get; set; }
        public int Score { get; set; }
    }
}
