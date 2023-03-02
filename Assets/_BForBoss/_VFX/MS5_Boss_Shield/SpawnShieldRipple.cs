using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace BForBoss
{
    public class SpawnShieldRipple : MonoBehaviour
    {
        public GameObject shieldRipples;

        private VisualEffect shieldRipplesVFX;

        private void OnCollisionEnter(Collision collision)
        {
            var ripples = Instantiate(shieldRipples, transform) as GameObject;
            shieldRipplesVFX = ripples.GetComponent<VisualEffect>();
            shieldRipplesVFX.SetVector3("SphereCenter", collision.contacts[0].point);
            
            Destroy(ripples, 2);
            
            Debug.Log("HIT!");
            Debug.Log(collision.contacts[0].point);
            Debug.Log(shieldRipplesVFX.GetVector3("SphereCenter"));
            
        }
    }
}
