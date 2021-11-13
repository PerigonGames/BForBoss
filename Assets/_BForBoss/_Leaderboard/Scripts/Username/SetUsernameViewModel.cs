using System;
using UnityEngine;

namespace BForBoss
{
    public class SetUsernameViewModel
    {
        private readonly InputUsername _inputUsername = new InputUsername();
        public event Action OnSuccess;
        public event Action OnFailure;

        public string Username => _inputUsername.Username;

        public void SetUserName(string username)
        {
            if (_inputUsername.CanUseThisUsername(username))
            {
                _inputUsername.SetUserName(username);
                OnSuccess?.Invoke();
            }
            else
            {
                OnFailure?.Invoke();
            }
        }

        public bool IsUsernameAlreadySet()
        {
            return PlayerPrefs.HasKey(UploadPlayerScoreDataSource.PlayerPrefKey.UserName);
        }

        public void RemoveSubscribers()
        {
            //OnSuccess = null;
            //OnFailure = null;
        }
    }
}