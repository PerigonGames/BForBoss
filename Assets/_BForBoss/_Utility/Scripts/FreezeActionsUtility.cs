namespace Perigon.Utility
{
    public class FreezeActionsUtility: ILockInput
    {
        private IInputSettings _input = null;
        
        public FreezeActionsUtility(IInputSettings input)
        {
            _input = input;
        }

        void ILockInput.LockInput()
        {
            LockMouseUtility.Instance.UnlockMouse();
            _input.SwapToUIActions();
        }

        void ILockInput.UnlockInput()
        {
            LockMouseUtility.Instance.LockMouse();
            _input.SwapToPlayerActions();
        }
    }
}