using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace BForBoss
{
    public class DELETETHIS : MonoBehaviour
    {
        private Material mat;
        private CustomPassVolume cps;
        private Color mainColor;

        [SerializeField] public float speed = 1;
        [SerializeField] public float strength = 1;
        // Start is called before the first frame update
        void Start()
        {
            cps = gameObject.GetComponent<CustomPassVolume>();
            
            foreach (var pass in cps.customPasses)
            {
                if (pass is FullScreenCustomPass f)
                {
                    mat = f.fullscreenPassMaterial;
                }
            }

        }

        // Update is called once per frame
        void Update()
        {
            float factor = (float)( Math.Abs(Math.Sin(Time.realtimeSinceStartup * (1 / speed))) * strength);
            mat.SetFloat("_EmissionStrength",factor ); 
        }
    }
}
