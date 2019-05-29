using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Dart.Messaging.Models
{
    public enum GameState
    {
        Pending,
        Completed,
        Deleted
    }
    public class Game
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid GamerId { get; set; }
        public int Score { get; set; }
        public GameState State { get; set; }

    }
}
