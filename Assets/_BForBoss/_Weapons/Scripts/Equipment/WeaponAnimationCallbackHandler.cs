using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perigon.Weapons
{
    public class WeaponAnimationCallbackHandler : MonoBehaviour
    {
        private WeaponsManager _manager;
        
        private void Start()
        {
            _manager = GetComponentInParent<WeaponsManager>();
        }

        // Called from animation events
        public void MeleeHitCallback()
        {
            _manager.MeleeHitCallback();
        }
    }
}
