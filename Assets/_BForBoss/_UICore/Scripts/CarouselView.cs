using System;
using Perigon.Utility;
using PerigonGames;
using UnityEngine;

namespace UICore
{
    public class CarouselView : MonoBehaviour
    {
        public Action OnExitAction;
        
        private CarouselPanelView[] _carouselPanels;

        public void Show()
        {
            ApplyToAllPanels(index =>
            {
                _carouselPanels[index].gameObject.SetActive(false);
            });
            gameObject.SetActive(true);
            _carouselPanels[0].gameObject.SetActive(true);
        }
        
        public void Initialize()
        {
            ApplyToAllPanels(index =>
            {
                _carouselPanels[index].gameObject.SetActive(false);
            });
            SetupPanels();
            SetupButtons();
        }

        private void Awake()
        {
            _carouselPanels = gameObject.GetComponentsInChildren<CarouselPanelView>(includeInactive: true);
            if (_carouselPanels.IsNullOrEmpty())
            {
                PanicHelper.Panic(new Exception("Carousel Panels missing from CarouselView"));
            }
        }

        private void SetupPanels()
        {
            if (_carouselPanels.Length == 1)
            {
                _carouselPanels[0].SetState(isBackShown: false, isContinueShown: false);
                return;
            }
            
            for (var i = 0; i < _carouselPanels.Length; i++)
            {
                if (i == 0)
                {
                    _carouselPanels[i].SetState(isBackShown: false, isContinueShown: true);
                }

                if (i == _carouselPanels.Length - 1)
                {
                    _carouselPanels[i].SetState(isBackShown:true, isContinueShown: false);
                }
            }
        }

        private void SetupButtons()
        {
            ApplyToAllPanels(index =>
            {
                _carouselPanels[index].BackButtonAction = () =>
                {
                    _carouselPanels[index].gameObject.SetActive(false);
                    _carouselPanels[Math.Max(0, index - 1)].gameObject.SetActive(true);
                };
            });
            
            ApplyToAllPanels(index =>
            {
                _carouselPanels[index].ContinueButtonAction = () =>
                {
                    _carouselPanels[index].gameObject.SetActive(false);
                    _carouselPanels[Math.Min(_carouselPanels.Length - 1, index + 1)].gameObject.SetActive(true);
                };
            });
            
            ApplyToAllPanels(index =>
            {
                _carouselPanels[index].ExitButtonAction = () =>
                {
                    _carouselPanels[index].gameObject.SetActive(false);
                    gameObject.SetActive(false);
                    OnExitAction?.Invoke();
                };
            });
        }

        private void ApplyToAllPanels(Action<int> lambda)
        {
            for (var i = 0; i < _carouselPanels.Length; i++)
            {
                lambda(i);
            }
        }
    }
}
