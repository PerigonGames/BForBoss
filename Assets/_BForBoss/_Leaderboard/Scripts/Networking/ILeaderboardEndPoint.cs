using System;

namespace Perigon.Leaderboard
{
    public interface ILeaderboardGetEndPoint
    {
        public void GetLeaderboard();
        public event Action<LeaderboardScore[]> OnSuccess;
        public event Action OnFail;
    }

    public interface ILeaderboardPostEndPoint
    {
        public void SendScore(string username, int milliseconds);
        public event Action OnSuccess;
        public event Action OnFail;
    }
}
