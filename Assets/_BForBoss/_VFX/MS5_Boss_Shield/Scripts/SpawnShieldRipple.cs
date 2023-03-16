using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace BForBoss
{
    public class SpawnShieldRipple : MonoBehaviour
    {
        [SerializeField]
        private GameObject shieldRipples;

        private VisualEffect _shieldRipplesVFX;

        private void OnCollisionEnter(Collision collision)
        {
            GameObject ripples = Instantiate(shieldRipples, transform);
            _shieldRipplesVFX = ripples.GetComponent<VisualEffect>();
            Vector3 collisionPoint = collision.contacts[0].point; 
            _shieldRipplesVFX.SetVector3("SphereCenter", collisionPoint);
            Destroy(ripples, 2);
        }
    }
}