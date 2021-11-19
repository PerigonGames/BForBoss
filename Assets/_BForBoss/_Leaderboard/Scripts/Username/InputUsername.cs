using System;
using System.Linq;
using System.Text.RegularExpressions;
using Sirenix.Utilities;
using UnityEngine;

namespace BForBoss
{
    public class InputUsername
    {
        public static readonly int CharacterLimit = 20;
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
            var notOnlyWhiteSpace = !username.IsNullOrWhitespace();
            var isWithinTwentyChar = username.Length < CharacterLimit;
            var onlyLetterOrDigits = IsAlphaNumeric(username);
            
            return notOnlyWhiteSpace && isWithinTwentyChar && onlyLetterOrDigits;
        }
        
        private bool IsAlphaNumeric(string inputString)
        {
            var regexPattern = "^[a-zA-Z0-9 ]+$";
            Regex r = new Regex(regexPattern);
            return r.IsMatch(inputString);
        }
    }
}