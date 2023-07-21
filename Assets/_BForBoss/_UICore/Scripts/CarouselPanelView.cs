using System;
using Perigon.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace UICore
{
    public class CarouselPanelView : MonoBehaviour
    {
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _exitButton;

        public Action BackButtonAction;
        public Action ContinueButtonAction;
        public Action ExitButtonAction;
        

        public void SetState(bool isBackShown, bool isContinueShown)
        {
            if (!isContinueShown)
            {
                _continueButton.gameObject.SetActive(false);
            }

            if (!isBackShown)
            {
                _backButton.gameObject.SetActive(false);
            }
        }

        private void Awake()
        {
            this.PanicIfNullObject(_backButton, nameof(_backButton));
            this.PanicIfNullObject(_continueButton, nameof(_continueButton));
            this.PanicIfNullObject(_exitButton, nameof(_exitButton));

            _backButton.onClick.AddListener(() =>
            {
                BackButtonAction?.Invoke();
            });
            
            _continueButton.onClick.AddListener(() =>
            {
                ContinueButtonAction?.Invoke();
            });
            
            _exitButton.onClick.AddListener(() =>
            {
                ExitButtonAction?.Invoke();
            });
        }

        private void OnDestroy()
        {
            _backButton.onClick.RemoveAllListeners();
            _continueButton.onClick.RemoveAllListeners();
            _exitButton.onClick.RemoveAllListeners();
        }
    }
}
