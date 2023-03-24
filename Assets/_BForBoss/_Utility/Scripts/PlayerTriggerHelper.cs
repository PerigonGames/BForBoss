using System;
using Perigon.Utility;
using UnityEngine;

namespace BForBoss.Utility
{
    public class PlayerTriggerHelper : MonoBehaviour
    {
        public event Action PlayerEnteredTrigger;
        
        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.layer == TagsAndLayers.Layers.Player)
                PlayerEnteredTrigger?.Invoke();
        }
    }
}
