using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perigon.AI
{
    public class DummyTargetBehaviour : LifeCycleBehaviour
    {
        protected override void LifeCycleFinished()
        {
            Destroy(gameObject);
        }
    }
}
