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
        public int HumanHighGoals { get; set; }
        public int HumanLowGoals { get; set; }

        // teleop
        public int TeleHighGoals { get; set; }
        public int TeleLowGoals { get; set; }

        // end game
        public int ClimbLevel { get; set; }

        // overall
        public int MatchRP { get; set; } // 0=Lost, 1=Tie, 2=Win
        public int CargoRP { get; set; }
        public int ClimbRP { get; set; }
        public int ScouterRating { get; set; }
        public string Comments { get; set; }
    }
}
