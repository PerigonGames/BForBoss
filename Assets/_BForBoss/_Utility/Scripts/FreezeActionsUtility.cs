namespace Perigon.Utility
{
    public class FreezeActionsUtility: ILockInput
    {
        private IInputSettings _input = null;
        
        public FreezeActionsUtility(IInputSettings input)
        {
            _input = input;
        }

        public void LockInput()
        {
            LockMouseUtility.Instance.UnlockMouse();
            _input.DisableActions();
        }

        public void UnlockInput()
        {
            LockMouseUtility.Instance.LockMouse();
            _input.EnableActions();
        }
    }
}