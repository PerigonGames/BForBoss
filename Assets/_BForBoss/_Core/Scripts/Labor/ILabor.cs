using System;

namespace BForBoss.Labor
{
    public class OnLaborCompletedArgs : EventArgs
    {
        public bool DidSucceed { get; }

        public OnLaborCompletedArgs(bool didSucceed)
        {
            DidSucceed = didSucceed;
        }
    }
    
    public delegate void OnLaborCompletedEventHandler(ILabor sender, OnLaborCompletedArgs completedArgs);
    
    public interface ILabor
    {
        public event OnLaborCompletedEventHandler OnLaborCompleted;

        public void Activate();

        public void Reset();
    }
}
