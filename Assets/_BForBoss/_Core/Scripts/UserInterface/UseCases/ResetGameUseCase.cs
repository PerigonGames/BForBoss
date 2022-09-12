namespace BForBoss
{
    public class ResetGameUseCase
    {
        private IStateManager _stateManager;
        
        public ResetGameUseCase(IStateManager stateManager)
        {
            _stateManager = stateManager;
        }
        
        public void Execute()
        {
            _stateManager.SetState(State.PreGame);
        }
    }
}
