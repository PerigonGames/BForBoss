using System;
using Perigon.Utility;
using PerigonGames;
using UnityEngine;

namespace BForBoss
{
    public class CarouselView : MonoBehaviour
    {
        private CarouselPanelView[] _carouselPanels;

        private void Awake()
        {
            _carouselPanels = gameObject.GetComponentsInChildren<CarouselPanelView>();
            if (_carouselPanels.IsNullOrEmpty())
            {
                PanicHelper.Panic(new Exception("Carousel Panels missing from CarouselView"));
            }
        }

        private void Start()
        {
            ApplyToAllPanels(index =>
            {
                _carouselPanels[index].Initialize(index: index);
            });
            ApplyToAllPanels(index =>
            {
                _carouselPanels[index].gameObject.SetActive(false);
            });
            SetupPanels();
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

        private void ApplyToAllPanels(Action<int> lambda)
        {
            for (var i = 0; i < _carouselPanels.Length; i++)
            {
                lambda(i);
            }
        }
    }
}
