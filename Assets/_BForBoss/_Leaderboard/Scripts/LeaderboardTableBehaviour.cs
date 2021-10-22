using System;
using UnityEngine;

namespace BForBoss
{
    public class LeaderboardTableBehaviour : MonoBehaviour
    {
        [SerializeField] private LeaderboardRowBehaviour[] LeaderboardRows;

        private DreamloGetLeaderboardEndPoint LeaderboardEndpoint = new DreamloGetLeaderboardEndPoint();
        private string Rank;

        // Start is called before the first frame update

        private void Awake()
        {
            LeaderboardEndpoint.OnSuccess += HandleOnSuccess;
        }

        private void HandleOnSuccess(LeaderboardScore[] scores)
        {
            Debug.Log("I finished getting data from the internet");

            for (var i = 0; i < 10; i++)
            {
                if(scores.Length > i)
                {
                    LeaderboardRows[i].SetField(i+1, scores[i]);
                }
                else
                {
                    LeaderboardRows[i].SetFieldEmpty();
                }
            }
        }

        private void OnEnable()
        {
            LeaderboardEndpoint.GetLeaderboard(); //Starts getting scores from database
        }
    }
}
