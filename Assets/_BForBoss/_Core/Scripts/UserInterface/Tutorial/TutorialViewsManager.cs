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
        Basic,
        Energy
    }
    
    public class TutorialViewsManager : MonoBehaviour
    {
        public static TutorialViewsManager Instance;

        [SerializeField] private CarouselView _energyTutorialView;
        [SerializeField] private CarouselView _basicTutorialView;

        private readonly Dictionary<TutorialState, bool> _shownTutorials = new Dictionary<TutorialState, bool>();

        private void Awake()
        {
            Instance = this;
            SetupTutorials();
        }

        private IStateManager StateManager => BForBoss.StateManager.Instance;

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

        private void SetupTutorials()
        {
            void ResumeGame()
            {
                StateManager.SetState(State.Play);
            }

            _energyTutorialView.OnExitAction = ResumeGame;
            _basicTutorialView.OnExitAction = ResumeGame;
        }

        private CarouselView MapToView(TutorialState state)
        {
            switch (state)
            {
                case TutorialState.Basic:
                    return _basicTutorialView;
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
