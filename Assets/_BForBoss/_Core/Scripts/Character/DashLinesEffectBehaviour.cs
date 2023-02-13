//-----------------------------------------------------------------------
// <copyright file="DashLinesEffectBehaviour.cs" company="PERIGON GAMES">
//     Copyright (c) PERIGON GAMES. 2020 All Rights Reserved
// </copyright>
//-----------------------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;

namespace BForBoss
{
    [RequireComponent(typeof(RawImage))]
    public class DashLinesEffectBehaviour : MonoBehaviour
    {
        private const string MATERIAL_ALPHA = "_alpha";
        
        [SerializeField] [Range(0,1)] float _lerpStrength = 0.1f;
        [SerializeField] float _duration = 0.1f;
        private float _currentTime;
        
        [SerializeField] [Range(0,1)] float _maxAlpha = 1f;
        private Material _speedLineMaterial;
        
        private void Start()
        {
            _speedLineMaterial = GetComponent<RawImage>().material;
            _speedLineMaterial.SetFloat(MATERIAL_ALPHA,  0);
            _currentTime = 0f;
        }

        private void Update()
        {
            var currentAlpha = _speedLineMaterial.GetFloat(MATERIAL_ALPHA);
            
            var isLerpingToZero = _currentTime <= 0;
            var resultingAlpha = isLerpingToZero ? 0 : _maxAlpha;

            _speedLineMaterial.SetFloat(MATERIAL_ALPHA, Mathf.Lerp(currentAlpha, resultingAlpha, _lerpStrength));

            _currentTime -= Time.deltaTime;

        }

        public void Play()
        {
            _currentTime = _duration;
        }

    }
}

