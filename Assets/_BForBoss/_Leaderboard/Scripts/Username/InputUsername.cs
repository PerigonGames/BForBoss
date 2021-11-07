using Sirenix.Utilities;
using UnityEngine;

namespace BForBoss
{
    public class InputUsername
    {
        private const int CharacterLimit = 20;
        private readonly IPerigonAnalytics _analytics = null;
        
        public string Username => PlayerPrefs.GetString(UploadPlayerScoreDataSource.PlayerPrefKey.UserName, "N/A");
        
        public InputUsername(IPerigonAnalytics analytics = null)
        {
            _analytics = analytics ?? PerigonAnalytics.Instance;
        }

        public void SetUserName(string username)
        {
            PlayerPrefs.SetString(UploadPlayerScoreDataSource.PlayerPrefKey.UserName, username);
            _analytics.SetUsername(username);
        }

        public bool CanUseThisUsername(string username)
        {
            var isWhiteSpace = !username.IsNullOrWhitespace();
            var isWithinTwentyChar = username.Length < CharacterLimit;

            return isWhiteSpace && isWithinTwentyChar;
        }
    }
}