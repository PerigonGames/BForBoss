using System;
using Perigon.Utility;
using Sirenix.Utilities;
using UnityEngine;

namespace Perigon.Leaderboard
{
    public class UploadPlayerScoreDataSource
    {
        

        private const int MaxNumberOfRetries = 3;
        private readonly ILeaderboardPostEndPoint _endpoint = null;
        private int _numberOfRetries = 0;

        private int _time = int.MaxValue;
        private string _input = "";

        public event Action StartUploading;
        public event Action StopLoading;

        private string Username => PlayerPrefs.GetString(PlayerPrefKeys.LeaderboardSettings.UserName, "");

        private int Time
        {
            get => _time;
            set
            {
                PlayerPrefs.SetFloat(PlayerPrefKeys.LeaderboardSettings.Timer, value);
                _time = value;
            }
        }

        private string Input
        {
            get => _input;
            set
            {
                PlayerPrefs.SetString(PlayerPrefKeys.LeaderboardSettings.Input, value);
                _input = value;
            }
        }
        private bool ShouldUploadScores => PlayerPrefs.GetInt(PlayerPrefKeys.LeaderboardSettings.ShouldUpload, 0) == 1;

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
                _time = PlayerPrefs.GetInt(PlayerPrefKeys.LeaderboardSettings.Timer, int.MaxValue);
                _input = PlayerPrefs.GetString(PlayerPrefKeys.LeaderboardSettings.Input, "");
            }
        }

        private void Upload()
        {
            PlayerPrefs.SetInt(PlayerPrefKeys.LeaderboardSettings.ShouldUpload, 1);
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
            PlayerPrefs.DeleteKey(PlayerPrefKeys.LeaderboardSettings.Timer);
            PlayerPrefs.DeleteKey(PlayerPrefKeys.LeaderboardSettings.Input);
            PlayerPrefs.DeleteKey(PlayerPrefKeys.LeaderboardSettings.ShouldUpload);
            StopLoading?.Invoke();
        }

        private void HandleEndPointOnFail()
        {
            _numberOfRetries++;
            Debug.Log("Number of Tries: "+_numberOfRetries);
            var isNumberOfRetriesWithinLimit = _numberOfRetries < MaxNumberOfRetries;
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