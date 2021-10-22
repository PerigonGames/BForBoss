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
        // Milliseconds
        public float seconds;
        public string text;
    }
}
