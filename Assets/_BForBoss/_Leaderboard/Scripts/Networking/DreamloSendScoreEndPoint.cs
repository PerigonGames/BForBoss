using System;
using System.Net.Http;

namespace Perigon.Leaderboard
{
    public class DreamloSendScoreEndPoint : ILeaderboardPostEndPoint
    {
        private string _username;
        private int _score;
        private int _milliseconds;
        private string _input;

        private event Action _onSuccess;
        private event Action _onFail;

        event Action ILeaderboardPostEndPoint.OnSuccess
        {
            add => _onSuccess += value;
            remove => _onSuccess -= value;
        }

        event Action ILeaderboardPostEndPoint.OnFail
        {
            add => _onFail += value;
            remove => _onFail -= value;
        }

        void ILeaderboardPostEndPoint.SendScore(string username, int milliseconds, string input)
        {
            _username = username;
            _score = -milliseconds;
            _milliseconds = milliseconds;
            _input = input;
            Post();
        }

        private string Path => $"{DreamloData.Host}{DreamloData.Secret}/add/{_username}/{_score}/{_milliseconds}/{_input}";

        private async void Post()
        {
            var client = new HttpClient();
            var response = await client.GetAsync(Path);
            if (response.IsSuccessStatusCode)
            {
                _onSuccess?.Invoke();
            }
            else
            {
                _onFail?.Invoke();
            }
        }
    }
}
