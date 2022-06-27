using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace BForBoss
{
    public class HitEffectDemo : MonoBehaviour
    {
        private Material mat;
        private CustomPassVolume cps;
        private Color mainColor;

        [SerializeField] public Boolean isOn = true;
        [SerializeField] public float speed = 1;
        [SerializeField] public float strength = 1;
        
        void Start()
        {
            cps = gameObject.GetComponent<CustomPassVolume>();
            
            foreach (var pass in cps.customPasses)
            {
                if (pass is FullScreenCustomPass f)
                    mat = f.fullscreenPassMaterial;
            }

        }

        void Update()
        {
            if (isOn)
            {
                float factor = (float)( Math.Abs(Math.Sin(Time.realtimeSinceStartup * (1 / speed))) * strength);
                mat.SetFloat("_EmissionStrength", factor);
            }
            else
            {
                mat.SetFloat("_EmissionStrength", 0f);
            }
        }
    }
}
