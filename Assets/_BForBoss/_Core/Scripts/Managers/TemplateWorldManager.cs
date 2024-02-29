namespace BForBoss
{
    public class TemplateWorldManager : BaseWorldManager
    {
        protected override void Start()
        {
            base.Start();
            _stateManager.SetState(State.PreGame);
        }
    }
}
