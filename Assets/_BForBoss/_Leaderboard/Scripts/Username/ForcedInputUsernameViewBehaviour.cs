using PerigonGames;
using TMPro;
using UnityEngine;

namespace BForBoss
{
    public class ForcedInputUsernameViewBehaviour : InputUsernameViewBehaviour
    {
        public override void Initialize(InputUsernameViewModel viewModel = null)
        {
            _viewModel = viewModel ?? new InputUsernameViewModel();
            base.Initialize(_viewModel);
            
            if (_viewModel.IsUsernameAlreadySet())
            {
                HidePanel();
            }
            else
            {
                ShowPanel();
            }
        }
        
        protected override void BindViewModel()
        {
            base.BindViewModel();
            _viewModel.OnSuccess += HidePanel;
        }
        
        private void HidePanel()
        {
            _infoSettingsLabel.text = "";
            transform.localScale = Vector3.zero;
        }

        private void ShowPanel()
        {
            transform.ResetScale();
        }
    }
}