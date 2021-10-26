using System;

namespace BForBoss
{
    [Serializable]
    public class LeaderboardDTO
    {
        public EntryDTO[] entry;
    }

    [Serializable]
    public class EntryDTO
    {
        public string name;
        public int score;
        public int seconds;
        public string text;
        public DateTime date;
    }
}
