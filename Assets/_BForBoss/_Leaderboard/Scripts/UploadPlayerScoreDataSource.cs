using System;
using Perigon.Utility;
using Sirenix.Utilities;
using UnityEngine;

namespace Perigon.Leaderboard
{
    public class UploadPlayerScoreDataSource
    {
        private const int MAX_NUMBER_OF_RETRIES = 3;
        private readonly ILeaderboardPostEndPoint _endpoint = null;
        private int _numberOfRetries = 0;

        private int _time = int.MaxValue;
        private string _input = "";

        public event Action StartUploading;
        public event Action StopLoading;

        private string Username => PlayerPrefs.GetString(PlayerPrefKeys.LeaderboardSettings.USERNAME, "");

        private int Time
        {
            get => _time;
            set
            {
                PlayerPrefs.SetFloat(PlayerPrefKeys.LeaderboardSettings.TIMER, value);
                _time = value;
            }
        }

        private string Input
        {
            get => _input;
            set
            {
                PlayerPrefs.SetString(PlayerPrefKeys.LeaderboardSettings.INPUT, value);
                _input = value;
            }
        }
        private bool ShouldUploadScores => PlayerPrefs.GetInt(PlayerPrefKeys.LeaderboardSettings.SHOULDUPLOAD, 0) == 1;

        public UploadPlayerScoreDataSource(ILeaderboardPostEndPoint endpoint = null)
        {
            SetupProperties();
            _endpoint = endpoint ?? new DreamloSendScoreEndPoint();
            _endpoint.OnSuccess += HandleEndPointOnSuccess;
            _endpoint.OnFail += HandleEndPointOnFail;

            UploadIfPossible();
        }

        public void UploadScoreIfPossible(int time, string input)
        {
            Time = Mathf.Min(time, Time);
            Input = input;
            UploadIfPossible();
        }
        
        
        private void UploadIfPossible()
        {
            if (CanUpload())
            {
                Upload();
                StartUploading?.Invoke();
            }
        }

        private void SetupProperties()
        {
            if (ShouldUploadScores)
            {
                _time = PlayerPrefs.GetInt(PlayerPrefKeys.LeaderboardSettings.TIMER, int.MaxValue);
                _input = PlayerPrefs.GetString(PlayerPrefKeys.LeaderboardSettings.INPUT, "");
            }
        }

        private void Upload()
        {
            PlayerPrefs.SetInt(PlayerPrefKeys.LeaderboardSettings.SHOULDUPLOAD, 1);
            _endpoint.SendScore(Username, _time, _input);
        }

        private bool CanUpload()
        {
            var isUserNameFilled = !Username.IsNullOrWhitespace();
            var isTimeHigher = _time < int.MaxValue;
            var isInputFilled = !_input.IsNullOrWhitespace();
            return isUserNameFilled && isTimeHigher && isInputFilled;
        }

        private void HandleEndPointOnSuccess()
        {
            _numberOfRetries = 0;
            PlayerPrefs.DeleteKey(PlayerPrefKeys.LeaderboardSettings.TIMER);
            PlayerPrefs.DeleteKey(PlayerPrefKeys.LeaderboardSettings.INPUT);
            PlayerPrefs.DeleteKey(PlayerPrefKeys.LeaderboardSettings.SHOULDUPLOAD);
            StopLoading?.Invoke();
        }

        private void HandleEndPointOnFail()
        {
            _numberOfRetries++;
            Debug.Log("Number of Tries: "+_numberOfRetries);
            var isNumberOfRetriesWithinLimit = _numberOfRetries < MAX_NUMBER_OF_RETRIES;
            if (isNumberOfRetriesWithinLimit)
            {
                UploadIfPossible();
            }
            else
            {
                StopLoading?.Invoke();
            }
        }
    }
}