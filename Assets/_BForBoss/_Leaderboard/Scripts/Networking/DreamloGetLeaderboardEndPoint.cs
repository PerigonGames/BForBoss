using System;
using System.Collections.Generic;
using System.Net.Http;
using Sirenix.Utilities;

namespace Perigon.Leaderboard
{
    public class DreamloGetLeaderboardEndPoint : ILeaderboardGetEndPoint
    {
        private event Action<LeaderboardScore[]> _onSuccess;
        private event Action _onFail;

        event Action<LeaderboardScore[]> ILeaderboardGetEndPoint.OnSuccess
        {
            add => _onSuccess += value;
            remove => _onSuccess -= value;
        }

        event Action ILeaderboardGetEndPoint.OnFail
        {
            add => _onFail += value;
            remove => _onFail -= value;
        }

        void ILeaderboardGetEndPoint.GetLeaderboard()
        {
            Get();
        }

        private async void Get()
        {
            var client = new HttpClient();
            var response = await client.GetAsync($"{DreamloData.Host}{DreamloData.Public}/quote-seconds-asc");
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                Parse(responseBody);
            }
            else
            {
                _onFail?.Invoke();
            }
        }

        private void Parse(string responseBody)
        {
            if (responseBody.IsNullOrWhitespace())
            {
                _onSuccess?.Invoke(new LeaderboardScore[]{});
            }
            else
            {
                var arrayOfEntryDTO = ParsingQuotedListToDTO(responseBody);
                var leaderboardScores = MapDTOToLeaderboardScore(arrayOfEntryDTO);
                _onSuccess?.Invoke(leaderboardScores);
            }
        }

        private LeaderboardScore[] MapDTOToLeaderboardScore(EntryDTO[] dto)
        {
            List<LeaderboardScore> listOfScore = new List<LeaderboardScore>();
            foreach (var scoreDTO in dto)
            {
                LeaderboardScore score = new LeaderboardScore();
                score.Username = scoreDTO.Username;
                score.Time = scoreDTO.MilliSeconds / 1000;
                score.Input = scoreDTO.Input;
                score.Date = scoreDTO.CreationDate;
                listOfScore.Add(score);
            }

            return listOfScore.ToArray();
        }

        #region Helper
        /* Response:
            "Amy","0","49124","Mouse + Keyboard","10/27/2021 11:13:49 PM"
            "bob","0","50244","Mouse + Keyboard","10/27/2021 11:13:43 PM"
         */
        private EntryDTO[] ParsingQuotedListToDTO(string source)
        {
            var quotedEntries = CleanQuotedSource(source);
            var entries = new List<EntryDTO>();
            foreach(var entry in quotedEntries)
            {
                if (!entry.IsNullOrWhitespace())
                {
                    var stringTypeEntry = entry.Split(',');
                    var entryDTO = buildEntryDTO(stringTypeEntry);
                    entries.Add(entryDTO);
                }
            }

            return entries.ToArray();
        }

        private String[] CleanQuotedSource(string source)
        {
            return source.Replace("\"", "").Split('\n');
        }

        /*
         * "bob","0","50244","Mouse + Keyboard","10/27/2021 11:13:43 PM"
         */
        private EntryDTO buildEntryDTO(string[] components)
        {
            try
            {
                var entryDTO = new EntryDTO();
                entryDTO.Username = components[0];
                entryDTO.MilliSeconds = int.Parse(components[2]);
                entryDTO.Input = components[3];
                entryDTO.CreationDate = Convert.ToDateTime(components[4]);
                return entryDTO;
            }
            catch (Exception _)
            {
                return new EntryDTO();
            }

        }
        #endregion
    }
}
