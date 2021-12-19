using SQLite;

// remember to increment the dbVersion in BertScout2022Database when changing this model

namespace BertScout2022.Data.Models
{
    public class TeamMatch
    {
        // metadata
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }
        public string Uuid { get; set; }
        [Indexed]
        public int TeamNumber { get; set; }
        [Indexed]
        public int MatchNumber { get; set; }
        public string ScouterName { get; set; }

        // autonomous
        public bool MovedOffStart { get; set; }
        public int AutoHighGoals { get; set; }
        public int AutoLowGoals { get; set; }

        // teleop
        public int TeleHighGoals { get; set; }
        public int TeleLowGoals { get; set; }

        // end game
        public bool Climbed { get; set; }

        // overall
        public bool Won { get; set; }
        public bool Tied { get; set; }
        public bool Lost { get; set; }
        public bool SpecialRP1 { get; set; }
        public bool SpecialRP2 { get; set; }
        public string Comments { get; set; }
    }
}
