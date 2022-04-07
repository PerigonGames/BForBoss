using System;
using System.Net.Http;

namespace Perigon.Leaderboard
{
    public class DreamloSendScoreEndPoint : ILeaderboardPostEndPoint
    {
        private string _username;
        private int _score;
        private int _milliseconds;

        public event Action OnSuccess;
        public event Action OnFail;

        public void SendScore(string username, int milliseconds)
        {
            _username = username;
            _score = -milliseconds;
            _milliseconds = milliseconds;
            Post();
        }

        private string Path => $"{DreamloData.Host}{DreamloData.Secret}/add/{_username}/{_score}/{_milliseconds}";

        private async void Post()
        {
            var client = new HttpClient();
            var response = await client.GetAsync(Path);
            if (response.IsSuccessStatusCode)
            {
                OnSuccess?.Invoke();
            }
            else
            {
                OnFail?.Invoke();
            }
        }
    }
}
