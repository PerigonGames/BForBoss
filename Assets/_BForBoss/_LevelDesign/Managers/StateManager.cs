using System;

namespace BForBoss
{
    public enum State
    {
        PreGame,
        Play,
        Pause,
        Death,
    }
    
    public class StateManager
    {
        private static readonly StateManager _instance = new StateManager();
        private State _currentState = State.PreGame;

        public Action<State> OnStateChanged;

        public static StateManager Instance => _instance;
        
        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static StateManager()
        {
        }

        private StateManager()
        {
        }

        public State GetState()
        {
            return _currentState;
        }
        
        public void SetState(State newState)
        {
            _currentState = newState;
            OnStateChanged?.Invoke(_currentState);
        }
    }
}
