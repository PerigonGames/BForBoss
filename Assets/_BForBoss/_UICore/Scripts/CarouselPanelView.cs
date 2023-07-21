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
            if (_backButton == null)
            {
                PanicHelper.Panic(new Exception("Back Button missing from carousel panel view"));
            }

            if (_continueButton == null)
            {
                PanicHelper.Panic(new Exception("Continue Button missing from carousel panel view"));
            }

            if (_exitButton == null)
            {
                PanicHelper.Panic(new Exception("Exit Button missing from carousel panel view"));
            }
            
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
    }
}
