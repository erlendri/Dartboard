using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dart.Web.Models
{
    public class Game
    {
        public Guid Id { get; set; }
        public Guid GamerId { get; set; }
        public DateTime StartTime { get; set; }
        public int? ScoreThrow1 { get; set; }
        public int? ScoreThrow2 { get; set; }
        public int? ScoreThrow3 { get; set; }

        public int TotalScore => (ScoreThrow1 ?? 0) + (ScoreThrow2 ?? 0) + (ScoreThrow3 ?? 0);

        public int SequenceNo { get; set; }
        public bool IsCurrent { get; set; }
    }
}