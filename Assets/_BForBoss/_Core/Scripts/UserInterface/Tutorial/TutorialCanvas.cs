using System;
using Perigon.Utility;
using Sirenix.OdinInspector;
using UICore;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace BForBoss
{
    public class TutorialCanvas : MonoBehaviour
    {
        [Title("Buttons")] 
        [Resolve][SerializeField] private Button _backButton;
        [Resolve][SerializeField] private Button _controlsButton;
        [Resolve][SerializeField] private Button _energyButton;
        [Resolve][SerializeField] private Button _bossButton;

        [Title("Tutorial Panels")]
        [Resolve] [SerializeField] private CarouselView _controlsTutorial;
        [Resolve] [SerializeField] private CarouselView _energyTutorial;
        [Resolve] [SerializeField] private CarouselView _bossTutorial;
        private Action OnBackPressed;
        
        public void Initialize(Action onBackPressed)
        {
            OnBackPressed = onBackPressed;
            this.PanicIfNullObject(_backButton, nameof(_backButton));
            this.PanicIfNullObject(_controlsButton, nameof(_controlsButton));
            this.PanicIfNullObject(_energyButton, nameof(_energyButton));
            this.PanicIfNullObject(_bossButton, nameof(_bossButton));
            
            ForEachTutorialCarouselView(delegate(CarouselView view)
            {
                view.Initialize();
                view.gameObject.SetActive(false);
                view.OnExitAction = () =>
                {
                    view.gameObject.SetActive(false);
                };
            });

            BindButtons();
        }

        private void BindButtons()
        {
            _controlsButton.onClick.AddListener(() =>
            {
                _controlsTutorial.Show();
            });
            _energyButton.onClick.AddListener(() =>
            {
                _energyTutorial.Show();
            });
            _bossButton.onClick.AddListener(() =>
            {
                _bossTutorial.Show();
            });
            _backButton.onClick.AddListener(() =>
            {
                OnBackPressed?.Invoke();
            });
        }

        private void ForEachTutorialCarouselView(Action<CarouselView> action)
        {
            var views = new[] {_controlsTutorial, _energyTutorial, _bossTutorial};
            foreach (var view in views)
            {
                action?.Invoke(view);
            }
        }

        private void OnDisable()
        {
            ForEachTutorialCarouselView(delegate(CarouselView view)
            {
                view.Reset();
            });
        }
    }
}
