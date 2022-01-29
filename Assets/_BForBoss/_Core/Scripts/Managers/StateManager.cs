using System;

namespace BForBoss
{
    public enum State
    {
        PreGame,
        Play,
        Pause,
        EndRace,
        Death,
    }

    public interface IStateManager
    {
        State GetState();
        void SetState(State newState);
        
        Action<State> OnStateChanged { get; set; }
    }
    
    public class StateManager : IStateManager
    {
        private static readonly StateManager _instance = new StateManager();
        private State _currentState = State.PreGame;

        Action<State> IStateManager.OnStateChanged { get; set; }

        public static IStateManager Instance => _instance;
        
        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static StateManager()
        {
        }

        private StateManager()
        {
        }

        State IStateManager.GetState()
        {
            return _currentState;
        }

        void IStateManager.SetState(State newState)
        {
            _currentState = newState;
            Instance.OnStateChanged?.Invoke(_currentState);
        }
    }
}
