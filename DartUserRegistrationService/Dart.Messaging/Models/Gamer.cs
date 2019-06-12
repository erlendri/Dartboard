using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        [DisplayName("QrCode")]
        [Required]
        public string QrCode { get; set; }

        public List<Game> Games { get; set; } = new List<Game>();

        public GamerMinimal ToMinimal()
        {
            return new GamerMinimal
            {
                Id = Id,
                Name = DisplayName,
                MaxTries = 3,
                Tries = 0
            };
        }
       
    }

    public class GamerMinimal
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "maxTries")]
        public int MaxTries { get; set; }
        [JsonProperty(PropertyName = "count")]
        public int Tries { get; set; }
        [JsonProperty(PropertyName = "score")]
        public int Score { get; set; }
    }
}
