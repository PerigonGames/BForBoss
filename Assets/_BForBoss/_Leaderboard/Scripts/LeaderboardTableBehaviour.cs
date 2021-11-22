using UnityEngine;

namespace Perigon.Leaderboard
{
    public class LeaderboardTableBehaviour : MonoBehaviour
    {
        [SerializeField] private LeaderboardRowBehaviour[] _leaderboardRows;

        public void SetScores(LeaderboardScore[] scores)
        {
            for (var i = 0; i < 10; i++)
            {
                if(scores.Length > i)
                {
                    _leaderboardRows[i].SetField(i+1, scores[i]);
                }
            }
        }
    }
}
