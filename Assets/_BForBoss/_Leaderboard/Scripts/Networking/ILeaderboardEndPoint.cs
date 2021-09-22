using System;

namespace BForBoss
{
    public interface ILeaderboardGetEndPoint
    {
        public void GetLeaderboard();
        public event Action<LeaderboardScore[]> OnSuccess;
        public event Action OnFail;
    }

    public interface ILeaderboardPostEndPoint
    {
        public void SendScore(string username, float milliseconds, string input);
        public event Action OnSuccess;
        public event Action OnFail;
    }
}
