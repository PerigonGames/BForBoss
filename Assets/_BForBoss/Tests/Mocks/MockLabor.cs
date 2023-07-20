using System;
using System.Collections;
using System.Collections.Generic;
using BForBoss.Labor;
using UnityEngine;

namespace Tests
{
    public class MockLabor : ILabor
    {
        public bool IsActivated { get; private set; }
        
        public event Action<bool> OnLaborCompleted;
        
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
                OnLaborCompleted?.Invoke(true);
            }
        }
        
        public void Reset()
        {
            IsActivated = false;
        }
    }
}
