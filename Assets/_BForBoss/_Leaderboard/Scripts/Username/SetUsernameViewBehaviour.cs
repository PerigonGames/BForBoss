using Perigon.Utility;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Perigon.Leaderboard
{
    /// <summary>
    /// Placeholder view just to input the name
    /// </summary>
    public class SetUsernameViewBehaviour : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _usernameField = null;
        [SerializeField] protected TMP_Text _infoSettingsLabel = null;
        [SerializeField] private Button _setUsernameButton = null;
        protected SetUsernameViewModel _viewModel = null;
        
        public virtual void Initialize(ILockInput lockInput = null)
        {
            _viewModel = new SetUsernameViewModel();
            BindViewModel();
            _setUsernameButton.onClick.AddListener(() =>
            {
                _viewModel.SetUserName(_usernameField.text);
            });
        }

        protected virtual void BindViewModel()
        {
            _viewModel.OnFailure += ShowFailedText;
            _viewModel.OnSuccess += ShowSuccessText;
        }

        private void ShowFailedText()
        {
            _infoSettingsLabel.text = $"Something is wrong with your username, try another one. (Only Alphanumberic characters,  No Blank Text or names over {InputUsername.CharacterLimit} characters)";
            _infoSettingsLabel.color = Color.red;
        }

        private void ShowSuccessText()
        {
            _infoSettingsLabel.text = $"Success";
            _infoSettingsLabel.color = Color.green;
        }

        private void OnEnable()
        {
            if (_viewModel != null)
            {
                _usernameField.text = _viewModel.Username;
            }
        }

        private void OnDestroy()
        {
            if (_viewModel != null)
            {
                _viewModel.RemoveSubscribers();
            }
            _setUsernameButton.onClick.RemoveAllListeners();
        }
    }
}
