using System;
using System.Collections.Generic;
using Perigon.Utility;
using Sirenix.OdinInspector;
using UICore;
using UnityEngine;

namespace BForBoss
{
    public enum TutorialState
    {
        Boss,
        Energy
    }
    
    public class TutorialViewsManager : MonoBehaviour
    {
        public static TutorialViewsManager Instance;

        [SerializeField] private CarouselView _energyTutorialView;
        [SerializeField] private CarouselView _bossTutorialView;

        private readonly Dictionary<TutorialState, bool> _shownTutorials = new Dictionary<TutorialState, bool>();
        
        private IStateManager StateManager => BForBoss.StateManager.Instance;

        [Button]
        public void ShowBoss()
        {
            Show(TutorialState.Boss);
        }
        
        [Button]
        public void ShowEnergy()
        {
            Show(TutorialState.Energy);
        }
        
        public void Show(TutorialState state)
        {
            if (StateManager == null)
            {
                PanicHelper.Panic(new Exception("StateManager missing from TutorialViewsManager"));
            }
            
            if (CanShowTutorial(state))
            {
                StateManager.SetState(State.Tutorial);
                MapToView(state).Show();
            }
        }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            SetupTutorials();
            _energyTutorialView.Initialize();
            _bossTutorialView.Initialize();
        }
        
        private void SetupTutorials()
        {
            void ResumeGame()
            {
                StateManager.SetState(State.Play);
            }

            _energyTutorialView.OnExitAction = ResumeGame;
            _bossTutorialView.OnExitAction = ResumeGame;
        }

        private CarouselView MapToView(TutorialState state)
        {
            switch (state)
            {
                case TutorialState.Boss:
                    return _bossTutorialView;
                case TutorialState.Energy:
                    return _energyTutorialView;
            }
            
            PanicHelper.Panic(new Exception("Missing Tutorial State within TutorialViewsManager"));
            return null;
        }

        private bool CanShowTutorial(TutorialState state)
        {
            if (_shownTutorials.ContainsKey(state))
            {
                return false;
            }

            _shownTutorials.Add(state, true);
            return true;
        }
    }
}
