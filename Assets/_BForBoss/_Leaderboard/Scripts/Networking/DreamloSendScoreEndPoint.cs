using System;
using System.Net.Http;

namespace BForBoss
{
    public class DreamloSendScoreEndPoint : ILeaderboardPostEndPoint
    {
        private string _username;
        private int _milliseconds;
        private string _input;

        public event Action OnSuccess;
        public event Action OnFail;
        
        public void SendScore(string username, int milliseconds, string input)
        {           
            _username = username;
            _milliseconds = milliseconds;
            _input = input;
            Post();
        }

        private string Path => $"{DreamloData.Host}{DreamloData.Secret}/add/{_username}/0/{_milliseconds}/{_input}";

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
