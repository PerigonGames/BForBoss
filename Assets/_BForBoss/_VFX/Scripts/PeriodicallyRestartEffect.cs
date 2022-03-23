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
    [ExecuteInEditMode]     // Remove this if you don't want the script to update in editor.
    public class PeriodicallyRestartEffect : MonoBehaviour
    {

        [SerializeField] private float _wait_time = 1.0f;     // Time between shots.
        [SerializeField] private float _curr_time = 0f;       // Current time in the timer.
        
        void Update()
        {
            _curr_time += Time.deltaTime;    // Increment timer.
            if (_curr_time >= _wait_time)     // If wait_time is reached:
            {
                _curr_time = 0;                          // Reset timer.
                GetComponent<VisualEffect>().Reinit();  // Reinitialize the VFX (do the effect).
            }
        }
    }
}
