using BForBoss.Labor;

namespace Tests
{
    public class MockLabor : ILabor
    {
        public bool IsActivated { get; private set; }
        
        public event OnLaborCompletedEventHandler OnLaborCompleted;
        
        public MockLabor()
        {
            IsActivated = false;
        }
        
        public void Activate()
        {
            IsActivated = true;
        }

        public void CompleteLabor()
        {
            if (IsActivated)
            {
                OnLaborCompleted?.Invoke(this, new OnLaborCompletedArgs(true));
            }
        }
        
        public void Reset()
        {
            IsActivated = false;
        }
    }
}
