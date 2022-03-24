//-----------------------------------------------------------------------
// <copyright file="PeriodicallyRestartEffect.cs" company="PERIGON GAMES">
//     Copyright (c) PERIGON GAMES. 2020 All Rights Reserved
// </copyright>
//-----------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace BForBoss
{
    [ExecuteInEditMode]
    public class PeriodicallyRestartEffect : MonoBehaviour
    {

        [SerializeField] private float _waitTime = 1.0f;    
        private float _currTime = 0f;      
        [SerializeField] private VisualEffect _muzzleFlash = null;      
        
        private void Update()
        {
            _currTime += Time.deltaTime;  
            if (_currTime >= _waitTime)
            {
                _currTime = 0;                 
                _muzzleFlash.Reinit();  
            }
        }
    }
}
