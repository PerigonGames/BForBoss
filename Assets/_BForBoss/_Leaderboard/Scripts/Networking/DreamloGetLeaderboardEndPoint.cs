using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using UnityEngine;

namespace BForBoss
{
    public class DreamloGetLeaderboardEndPoint : ILeaderboardGetEndPoint
    {
        public event Action<LeaderboardScore[]> OnSuccess;
        public event Action OnFail;

        public void GetLeaderboard()
        {
            Get();
        }

        private async void Get()
        {
            var client = new HttpClient();
            var response = await client.GetAsync($"{DreamloData.Host}{DreamloData.Public}/json");
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                Parse(responseBody);
            }
            else
            {
                OnFail?.Invoke(); 
            }
        }

        private void Parse(string responseBody)
        {
            string json = RemoveNJsonFields(responseBody, 2);
            var leaderboardDTO = JsonUtility.FromJson<LeaderboardDTO>(json);
            var leaderboard = MapDTOToLeaderboardScore(leaderboardDTO);
            OnSuccess?.Invoke(leaderboard);
        }

        private LeaderboardScore[] MapDTOToLeaderboardScore(LeaderboardDTO dto)
        {
            List<LeaderboardScore> listOfScore = new List<LeaderboardScore>();
            foreach (var scoreDTO in dto.entry)
            {
                LeaderboardScore score = new LeaderboardScore();
                score.Username = scoreDTO.name;
                score.Time = scoreDTO.seconds / 1000;
                score.Input = scoreDTO.text;
                score.Date = scoreDTO.date;
                listOfScore.Add(score);
            }

            return listOfScore.ToArray();
        }
        
        #region Helper
        private string RemoveNJsonFields(string source, int n)
        {
            n++;

            int index = source.TakeWhile(c => (n -= (c == '{' ? 1 : 0)) > 0).Count();

            return source.Substring(index, source.Length - (index + 2 - n));
        }
        #endregion
    }
}
