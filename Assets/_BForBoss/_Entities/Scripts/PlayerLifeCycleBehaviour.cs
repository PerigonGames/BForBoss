using System;
using UnityEngine;

namespace Perigon.Entities
{
    public class PlayerLifeCycleBehaviour : LifeCycleBehaviour
    {
        private Action _onDeathCallBack = null;
        public override void Initialize(Action onDeathCallback)
        {
            base.Initialize(onDeathCallback);
            _onDeathCallBack = onDeathCallback;
        }

        protected override void LifeCycleFinished()
        {
            _onDeathCallBack?.Invoke();
        }
    }
}
