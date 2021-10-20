using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BForBoss
{
    public class LeaderboardTableBehaviour : MonoBehaviour
    {
        [SerializeField] private LeaderboardRowBehaviour[] LeaderboardRows;

        private DreamloGetLeaderboardEndPoint LeaderboardEndpoint = new DreamloGetLeaderboardEndPoint();

        // Start is called before the first frame update
        void Start()
        {
            var score_1 = new LeaderboardScore("Apple", 123, "MKB", new System.DateTime());
            var score_2 = new LeaderboardScore("Banana", 456, "CNTLR", new System.DateTime());
            var score_3 = new LeaderboardScore("Orange", 789, "MKB", new System.DateTime());
            var score_4 = new LeaderboardScore("Blueberry", 101, "MKB", new System.DateTime());
            var score_5 = new LeaderboardScore("Tomato", 236, "CNTLR", new System.DateTime());
            var score_6 = new LeaderboardScore("Watermelon", 384, "MKB", new System.DateTime());
            var score_7 = new LeaderboardScore("Starfruit", 102, "MKB", new System.DateTime());
            var score_8 = new LeaderboardScore("Mango", 987, "MKB", new System.DateTime());
            var score_9 = new LeaderboardScore("Papaya", 659, "CNTLR", new System.DateTime());
            var score_10 = new LeaderboardScore("Raspberry", 920, "MKB", new System.DateTime());

            var array = new LeaderboardScore[10];

            array[0] = score_1;
            array[1] = score_2;
            array[2] = score_3;
            array[3] = score_4;
            array[4] = score_5;
            array[5] = score_6;
            array[6] = score_7;
            array[7] = score_8;
            array[8] = score_9;
            array[9] = score_10;

            for (var i=0; i < 10; i++)
            {
                LeaderboardRows[i].SetField(array[i]);
            }
            
        }

        private void OnEnable()
        {
            LeaderboardEndpoint.GetLeaderboard(); //Starts getting scores from database
        }
    }
}
