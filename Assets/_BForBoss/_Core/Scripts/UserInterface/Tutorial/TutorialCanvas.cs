using System;
using Perigon.Utility;
using Sirenix.OdinInspector;
using UICore;
using UnityEngine;
using UnityEngine.UI;

namespace BForBoss
{
    public class TutorialCanvas : MonoBehaviour
    {
        [Title("Buttons")] 
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _basicButton;
        [SerializeField] private Button _energyButton;
        [SerializeField] private Button _wallRunButton;
        [SerializeField] private Button _bossButton;

        [Title("Tutorial Panels")]
        [Resolve] [SerializeField] private CarouselView _basicTutorial;
        [Resolve] [SerializeField] private CarouselView _energyTutorial;
        [Resolve] [SerializeField] private CarouselView _wallRunTutorial;
        [Resolve] [SerializeField] private CarouselView _bossTutorial;
        private Action OnBackPressed;
        
        public void Initialize(Action onBackPressed)
        {
            OnBackPressed = onBackPressed;
            this.PanicIfNullObject(_backButton, nameof(_backButton));
            this.PanicIfNullObject(_basicButton, nameof(_basicButton));
            this.PanicIfNullObject(_energyButton, nameof(_energyButton));
            this.PanicIfNullObject(_wallRunButton, nameof(_wallRunButton));
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
            _basicButton.onClick.AddListener(() =>
            {
                //_basicTutorial.gameObject.SetActive(true);
                _basicTutorial.Show();
            });
            _energyButton.onClick.AddListener(() =>
            {
                //_energyTutorial.gameObject.SetActive(true);
                _energyTutorial.Show();
            });
            _wallRunButton.onClick.AddListener(() =>
            {
                //_wallRunTutorial.gameObject.SetActive(true);
                _wallRunTutorial.Show();
            });
            _bossButton.onClick.AddListener(() =>
            {
                //_bossTutorial.gameObject.SetActive(true);
                _bossTutorial.Show();
            });
            _backButton.onClick.AddListener(() =>
            {
                OnBackPressed?.Invoke();
            });
        }

        private void ForEachTutorialCarouselView(Action<CarouselView> action)
        {
            var views = new CarouselView[] {_basicTutorial, _energyTutorial, _wallRunTutorial, _bossTutorial};
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
