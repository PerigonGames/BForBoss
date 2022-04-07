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
        
        private bool ShouldUploadScores => PlayerPrefs.GetInt(PlayerPrefKeys.LeaderboardSettings.SHOULD_UPLOAD, 0) == 1;

        public UploadPlayerScoreDataSource(ILeaderboardPostEndPoint endpoint = null)
        {
            SetupProperties();
            _endpoint = endpoint ?? new DreamloSendScoreEndPoint();
            _endpoint.OnSuccess += HandleEndPointOnSuccess;
            _endpoint.OnFail += HandleEndPointOnFail;

            UploadIfPossible();
        }

        public void UploadScoreIfPossible(int time)
        {
            Time = Mathf.Min(time, Time);
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
            }
        }

        private void Upload()
        {
            PlayerPrefs.SetInt(PlayerPrefKeys.LeaderboardSettings.SHOULD_UPLOAD, 1);
            _endpoint.SendScore(Username, _time);
        }

        private bool CanUpload()
        {
            var isUserNameFilled = !Username.IsNullOrWhitespace();
            var isTimeHigher = _time < int.MaxValue;
            return isUserNameFilled && isTimeHigher;
        }

        private void HandleEndPointOnSuccess()
        {
            _numberOfRetries = 0;
            PlayerPrefs.DeleteKey(PlayerPrefKeys.LeaderboardSettings.TIMER);
            PlayerPrefs.DeleteKey(PlayerPrefKeys.LeaderboardSettings.SHOULD_UPLOAD);
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