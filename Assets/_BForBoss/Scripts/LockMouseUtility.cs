namespace BForBoss
{
    public class LockMouseUtility : ILockMouseInput
    {
        private readonly IInputSettings _inputSettings = null;

        public LockMouseUtility(IInputSettings settings)
        {
            _inputSettings = settings;
        }

        public void LockMouse()
        {
            _inputSettings.SetMouseLock(true);
        }

        public void UnlockMouse()
        {
            _inputSettings.SetMouseLock(false);
        }
    }
}