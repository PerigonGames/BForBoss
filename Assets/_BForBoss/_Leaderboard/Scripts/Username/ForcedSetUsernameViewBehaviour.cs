using PerigonGames;
using UnityEngine;

namespace BForBoss
{
    public class ForcedSetUsernameViewBehaviour : SetUsernameViewBehaviour
    {
        public override void Initialize(SetUsernameViewModel viewModel = null)
        {
            _viewModel = viewModel ?? new SetUsernameViewModel();
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