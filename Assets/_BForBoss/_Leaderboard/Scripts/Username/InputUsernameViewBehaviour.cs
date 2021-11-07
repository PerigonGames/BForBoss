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
        [SerializeField] protected TMP_Text _infoSettingsLabel = null;
        [SerializeField] private Button _saveUsernameButton = null;
        protected InputUsernameViewModel _viewModel = null;
        
        public virtual void Initialize(InputUsernameViewModel viewModel = null)
        {
            _viewModel = viewModel ?? new InputUsernameViewModel();
            BindViewModel();
            _saveUsernameButton.onClick.AddListener(() =>
            {
                _viewModel.SetUserName(_usernameField.text);
            });
        }

        protected virtual void BindViewModel()
        {
            _viewModel.OnFailure += ShowFailedText;
        }
        
        private void ShowFailedText()
        {
            _infoSettingsLabel.text =
                "Something is wrong with your username, try another one. (No Blank Text or names over 20 characters";
            _infoSettingsLabel.color = Color.red;
        }

        private void OnEnable()
        {
            if (_viewModel != null)
            {
                _usernameField.text = _viewModel.Username;
            }
        }

        protected virtual void OnDestroy()
        {
            _saveUsernameButton.onClick.RemoveAllListeners();
        }
    }
}
