//-----------------------------------------------------------------------
// <copyright file="TransformMover.cs" company="PERIGON GAMES">
//     Copyright (c) PERIGON GAMES. 2020 All Rights Reserved
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using PerigonGames;
using UnityEngine;
using UnityEngine.Serialization;

namespace Perigon.VFX
{
    public class DemoPatrolBehaviour : MonoBehaviour
    {

        [SerializeField] private Vector3 _firstPointToVisit;      
        [SerializeField] private Vector3 _secondPointToVisit;       
        [SerializeField] private float _moveSpeedBetweenPoints = 0.4f;   

        private bool _isGoingToFirstPoint = true;          
                                                      
        private void Update()
        {
            var objectPosition = gameObject.transform.position;  
            
            if (_isGoingToFirstPoint)
            {
              
                objectPosition = Vector3.Lerp(objectPosition, _firstPointToVisit, _moveSpeedBetweenPoints * Time.deltaTime);
              
                if (Vector3.Distance(objectPosition, _firstPointToVisit) <= 0.1f)
                {
                    _isGoingToFirstPoint = false;
                }
            }
         
            else
            {
                objectPosition = Vector3.Lerp(objectPosition, _secondPointToVisit, _moveSpeedBetweenPoints * Time.deltaTime);
                if (Vector3.Distance(objectPosition, _secondPointToVisit) <= 0.1f)
                {
                    _isGoingToFirstPoint = true;
                }
            }
            
            gameObject.transform.SetPositionAndRotation(objectPosition, gameObject.transform.rotation);

        }
    }
}
