using System;
using System.Collections.Generic;
using Perigon.Utility;
using UICore;
using UnityEngine;

namespace BForBoss
{
    public enum TutorialState
    {
        FirstTutorial,
        WallRun,
        Slide,
        Boss
    }
    
    public class TutorialViewsManager : MonoBehaviour
    {
        public static TutorialViewsManager Instance;

        [Resolve][SerializeField] private CarouselView _firstTutorial;
        [Resolve][SerializeField] private CarouselView _wallRunTutorial;
        [Resolve][SerializeField] private CarouselView _slideTutorial;
        [Resolve][SerializeField] private CarouselView _bossTutorial;

        private readonly Dictionary<TutorialState, bool> _shownTutorials = new Dictionary<TutorialState, bool>();
        
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

        public void ShowWallRun()
        {
            Show(TutorialState.WallRun);
        }

        public void ShowSlide()
        {
            Show(TutorialState.Slide);
        }

        public void ShowBoss()
        {
            Show(TutorialState.Boss);
        }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            SetupTutorials();
            _firstTutorial.Initialize();
            _wallRunTutorial.Initialize();
            _slideTutorial.Initialize();
            _bossTutorial.Initialize();
        }
        
        private void SetupTutorials()
        {
            void ResumeGame()
            {
                StateManager.SetState(State.Play);
            }

            _firstTutorial.OnExitAction = ResumeGame;
            _wallRunTutorial.OnExitAction = ResumeGame;
            _slideTutorial.OnExitAction = ResumeGame;
            _bossTutorial.OnExitAction = ResumeGame;
        }

        private CarouselView MapToView(TutorialState state)
        {
            switch (state)
            {
                case TutorialState.FirstTutorial:
                    return _firstTutorial;
                case TutorialState.WallRun:
                    return _wallRunTutorial;
                case TutorialState.Slide:
                    return _slideTutorial;
                case TutorialState.Boss:
                    return _bossTutorial;
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
