using Perigon.Utility;
using PerigonGames;
using UnityEngine;

namespace Perigon.Leaderboard
{
    public class ForcedSetUsernameViewBehaviour : SetUsernameViewBehaviour
    {
        private ILockInput _lockInput = null;
        
        public override void Initialize(ILockInput lockInput = null)
        {
            _lockInput = lockInput;
            base.Initialize(lockInput);
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
            _lockInput.UnlockInput();
        }

        private void ShowPanel()
        {
            transform.ResetScale();
            _lockInput.LockInput();
        }
    }
}