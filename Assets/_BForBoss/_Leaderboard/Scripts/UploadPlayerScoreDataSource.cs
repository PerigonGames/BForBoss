using System;
using Sirenix.Utilities;
using UnityEngine;

namespace BForBoss
{
    public class UploadPlayerScoreDataSource
    {
        public struct PlayerPrefKey
        {
            public const string UserName = "UserName";
            public const string Timer = "Timer";
            public const string Input = "Input";
            public const string ShouldUpload = "ShouldUpload";
        }

        private const int MaxNumberOfRetries = 3;
        private readonly ILeaderboardPostEndPoint _endpoint = null;
        private int _numberOfRetries = 0;

        private int _time = int.MaxValue;
        private string _input = "";

        public event Action StartUploading;
        public event Action StopLoading;

        public string Username => PlayerPrefs.GetString(PlayerPrefKey.UserName, "");

        public int Time
        {
            get => _time;
            set
            {
                PlayerPrefs.SetFloat(PlayerPrefKey.Timer, value);
                _time = value;
            }
        }

        public string Input
        {
            get => _input;
            set
            {
                PlayerPrefs.SetString(PlayerPrefKey.Input, value);
                _input = value;
            }
        }
        private bool ShouldUploadScores => PlayerPrefs.GetInt(PlayerPrefKey.ShouldUpload, 0) == 1;

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
                _time = PlayerPrefs.GetInt(PlayerPrefKey.Timer, int.MaxValue);
                _input = PlayerPrefs.GetString(PlayerPrefKey.Input, "");
            }
        }

        private void Upload()
        {
            PlayerPrefs.SetInt(PlayerPrefKey.ShouldUpload, 1);
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
            PlayerPrefs.DeleteKey(PlayerPrefKey.Timer);
            PlayerPrefs.DeleteKey(PlayerPrefKey.Input);
            PlayerPrefs.DeleteKey(PlayerPrefKey.ShouldUpload);
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