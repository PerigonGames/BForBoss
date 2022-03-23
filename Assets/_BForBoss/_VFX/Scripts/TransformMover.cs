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

namespace BForBoss
{
    public class TransformMover : MonoBehaviour
    {

        [SerializeField] private Vector3 _point1;       // First point to visit.
        [SerializeField] private Vector3 _point2;       // Second point to visit.
        [SerializeField] private float _speed = 0.4f;   // The speed at with the transform moves between the two points

        private bool _isGoingToPoint1 = true;           // This is true if the transform is moving towards _point1,
                                                        // and false if moving towards _point2
        void Update()
        {
            var _position = gameObject.transform.position;  // Get the current position each frame.

            // If we are going to _point1, do the following
            if (_isGoingToPoint1)
            {
                // Set a new position by linearly interpolating between the current position and
                // the target position (_point1) based on _speed.
                _position = Vector3.Lerp(_position, _point1, _speed * Time.deltaTime);
                // If _point1 is reached, start moving towards _point2
                if (Vector3.Distance(_position, _point1) <= 0.1f)
                {
                    _isGoingToPoint1 = false;
                }
            }
            // If we are going to _point2, do the following
            else
            {
                _position = Vector3.Lerp(_position, _point2, _speed * Time.deltaTime);
                if (Vector3.Distance(_position, _point2) <= 0.1f)
                {
                    _isGoingToPoint1 = true;
                }
            }
            
            // Set the transform based on the above calculated position.
            gameObject.transform.SetPositionAndRotation(_position, gameObject.transform.rotation);

        }
    }
}
