﻿namespace Football.Models
{
    using System;

    public class Match
    {
        public int Id { get; set; }

        public virtual Team HomeTeam { get; set; }
        
        public virtual Team AwayTeam { get; set; }
        
        public virtual Stadium Stadium { get; set; }
        
        public int Attendance { get; set; }

        public int HomeScore { get; set; }

        public int AwayScore { get; set; }

        public DateTime Date { get; set; }
    }
}