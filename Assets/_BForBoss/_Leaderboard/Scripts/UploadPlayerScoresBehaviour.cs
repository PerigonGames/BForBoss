using System;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BForBoss
{
    public class UploadPlayerScoresBehaviour : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _usernameField = null;
        [SerializeField] private Button _uploadButton;

        private UploadPlayerScoresViewModel _viewModel = null;
        
        public void Initialize(UploadPlayerScoresViewModel viewModel)
        {
            _viewModel = viewModel;
            _usernameField.text = _viewModel.UserName;
        }
        
        private void Awake()
        {
            _uploadButton.onClick.AddListener(() =>
            {
                OnUploadPressed();
            });
        }

        private void OnDestroy()
        {
            _uploadButton.onClick.RemoveAllListeners();
        }

        private void OnUploadPressed()
        {
            _viewModel.UserName = _usernameField.text;
            _viewModel.UploadIfPossible();
        }
    }

    public class UploadPlayerScoresViewModel
    {
        private struct PlayerPrefKey
        {
            public const string UserName = "UserName";
            public const string Timer = "Timer";
            public const string Input = "Input";
        }
        
        private const int MaxNumberOfRetries = 3;
        private readonly ILeaderboardPostEndPoint _endpoint = null;        
        private int _numberOfRetries = 0;
        
        private string _username = "";
        private float _time = 0;
        private string _input = "";

        public event Action StartLoading;
        public event Action StopLoading;

        public string UserName
        {
            get => _username;

            set
            {
                PlayerPrefs.SetString(PlayerPrefKey.UserName, value);
                _username = value;
            }
        }

        public float Time
        {
            get => _time;
            set
            {
                PlayerPrefs.SetFloat(PlayerPrefKey.Timer, value);
                _time = value;
            }
        }

        public UploadPlayerScoresViewModel(ILeaderboardPostEndPoint endpoint = null)
        {
            SetupProperties();
            _endpoint = endpoint ?? new DreamloSendScoreEndPoint();
            _endpoint.OnSuccess += HandleEndPointOnSuccess;
            _endpoint.OnFail += HandleEndPointOnFail;

            UploadIfPossible();
        }


        private void Upload()
        {
            _endpoint.SendScore(_username, _time, _input);
        }
        
        public void UploadIfPossible()
        {
            if (CanUpload())
            {
                Upload();
                StartLoading?.Invoke();
            }
        }

        private bool CanUpload()
        {
            var isUserNameFilled = !_username.IsNullOrWhitespace();
            var isTimeHigher = _time > 0;
            var isInputFilled = !_input.IsNullOrWhitespace();
            return isUserNameFilled && isTimeHigher && isInputFilled;
        }
        
        private void SetupProperties()
        {
            _username = PlayerPrefs.GetString(PlayerPrefKey.UserName, "");
            _time = PlayerPrefs.GetFloat(PlayerPrefKey.Timer, 0);
            _input = PlayerPrefs.GetString(PlayerPrefKey.Input, "");
        }

        private void HandleEndPointOnSuccess()
        {
            PlayerPrefs.DeleteKey(PlayerPrefKey.Timer);
            PlayerPrefs.DeleteKey(PlayerPrefKey.Input);
            StopLoading?.Invoke();
        }

        private void HandleEndPointOnFail()
        {
            _numberOfRetries++;
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
