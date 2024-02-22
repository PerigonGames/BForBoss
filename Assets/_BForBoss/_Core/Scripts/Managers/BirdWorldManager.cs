namespace BForBoss
{
    public class BirdWorldManager : BaseWorldManager
    {
        protected override void Start()
        {
            base.Start();
            _stateManager.SetState(State.PreGame);
        }
    }
}
