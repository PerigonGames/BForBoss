using System;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BForBoss
{
    /// <summary>
    /// Placeholder view just to input the name
    /// </summary>
    public class InputUsernameViewBehaviour : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _usernameField = null;
        [SerializeField] private TMP_Text _infoSettingsLabel = null;
        [SerializeField] private Button _saveUsernameButton = null;
        private InputUsernameViewModel _viewModel = null;
        
        private void BindViewModel()
        {
            _viewModel.OnSuccess += () =>
            {
                HidePanel();
            };

            _viewModel.OnFailure += () =>
            {
                ShowFailedPanel();
            };
        }

        private void HidePanel()
        {
            transform.localScale = Vector3.zero;
        }

        private void ShowFailedPanel()
        {
            _infoSettingsLabel.text =
                "Something is wrong with your username, try another one. (No Blank Text or names over 20 characters";
            _infoSettingsLabel.color = Color.red;
        }
        
        private void Awake()
        {
            _viewModel = new InputUsernameViewModel();
            if (_viewModel.IsUsernameAlreadySet())
            {
                HidePanel();
            }
            
            _saveUsernameButton.onClick.AddListener(() =>
            {
                _viewModel.SetUserName(_usernameField.text);
            });
            
            BindViewModel();
        }
        
        private void OnDestroy()
        {
            _saveUsernameButton.onClick.RemoveAllListeners();
        }
    }

    public class InputUsernameViewModel
    {
        private const int CharacterLimit = 20;
        
        public event Action OnSuccess;
        public event Action OnFailure;
        
        public bool IsUsernameAlreadySet()
        {
            return PlayerPrefs.HasKey(UploadPlayerScores.PlayerPrefKey.UserName);
        }

        public void SetUserName(string username)
        {
            if (CanUseThisUsername(username))
            {
                PlayerPrefs.SetString(UploadPlayerScores.PlayerPrefKey.UserName, username);
                OnSuccess?.Invoke();
            }
            else
            {
                OnFailure?.Invoke();
            }
        }

        private bool CanUseThisUsername(string username)
        {
            var isWhiteSpace = !username.IsNullOrWhitespace();
            var isWithinTwentyChar = username.Length < CharacterLimit;

            return isWhiteSpace && isWithinTwentyChar;
        }
    }
}
