using PerigonGames;
using UnityEngine;

namespace BForBoss
{
    public class ForcedSetUsernameViewBehaviour : SetUsernameViewBehaviour
    {
        public override void Initialize(SetUsernameViewModel viewModel = null)
        {
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

        protected override void OnDestroy()
        {
            if (_viewModel != null)
            {
                _viewModel.OnSuccess -= HidePanel;
            }
            base.OnDestroy();
        }
    }
}