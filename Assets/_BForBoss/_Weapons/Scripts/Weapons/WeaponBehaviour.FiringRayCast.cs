using System.Collections;
using System.Collections.Generic;
using Perigon.Utility;
using UnityEngine;

namespace Perigon.Weapons
{
    public abstract partial class WeaponBehaviour
    {
        private void FireRayCastBullets(int numberOfBullets)
        {
            var camOrigin = MainCamera.ViewportPointToRay(CenterOfCameraPosition);
            if (Physics.Raycast(camOrigin,  out var hit, Mathf.Infinity, ~TagsAndLayers.Layers.TriggerArea))
            {
                Debug.Log("Ray Cast");
            }
        }
    }
}
