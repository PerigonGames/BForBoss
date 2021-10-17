using System;

namespace BForBoss
{
    public struct LeaderboardScore
    {
        public string Username;
        public float Time;
        public string Input;
        public DateTime Date;

        public LeaderboardScore(string username, float time, string input, DateTime date)
        {
            this.Username = username;
            this.Time = time;
            this.Input = input;
            this.Date = date;
        }
    }
}