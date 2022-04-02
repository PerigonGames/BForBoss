//-----------------------------------------------------------------------
// <copyright file="DashLinesEffectBehaviour.cs" company="PERIGON GAMES">
//     Copyright (c) PERIGON GAMES. 2020 All Rights Reserved
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Perigon.Character
{
    public class DashLinesEffectBehaviour : MonoBehaviour
    {
        [SerializeField] [Range(0,1)] float _lerpInStrength = 0.1f;
        [SerializeField] [Range(0,1)] float _lerpOutStrength = 0.1f;
        [SerializeField] float _duration = 0.1f;
        private float _currentTime;
        
        [SerializeField] [Range(0,1)] float _maxAlpha = 1f;
        private Material _speedLineMaterial;
        
        private void Start()
        {
            _speedLineMaterial = GetComponent<RawImage>().material;
            _speedLineMaterial.SetFloat("_alpha",  0);
            _currentTime = _duration + 10f;
        }

        private void Update()
        {
            var currentAlpha = _speedLineMaterial.GetFloat("_alpha");
            
            if (_currentTime > _duration)
            {
                _speedLineMaterial.SetFloat("_alpha",  Mathf.Lerp(currentAlpha, 0, _lerpOutStrength));
            }
            else
            {
                _speedLineMaterial.SetFloat("_alpha",  Mathf.Lerp(currentAlpha, _maxAlpha, _lerpInStrength));
            }

            _currentTime += Time.deltaTime;
        }

        public void Play()
        {
            _currentTime = 0f;
        }

    }
}

