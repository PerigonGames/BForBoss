using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perigon.Utility
{
    public class RotateToFacePlayer : MonoBehaviour
    {
        private static Transform _playerTransform;
        
        [SerializeField] private bool _lockYRotation = false;
        [SerializeField] private bool _lockXRotation = false;
        [SerializeField] private bool _lockZRotation = false;
        
        [SerializeField, Tooltip("Player must be further away than this distance in order to activate billboarding effect")] 
        private float _minDistanceToRotate = 5;

        private Vector3 _initialRotation;
        
        private void Awake()
        {
            FetchPlayer();
        }

        private void Start()
        {
            _initialRotation = transform.rotation.eulerAngles;
        }

        private void Update()
        {
            if(_playerTransform == null) return;
            
            Vector3 targetRotation = _playerTransform.position - transform.position;
            if(targetRotation.magnitude < _minDistanceToRotate) return;
            
            if(!_lockYRotation && !_lockXRotation && !_lockZRotation)
                transform.LookAt(_playerTransform);
            else
            {
                targetRotation.y = _lockYRotation ? _initialRotation.y : targetRotation.y;
                targetRotation.x = _lockXRotation ? _initialRotation.x : targetRotation.x;
                targetRotation.z = _lockZRotation ? _initialRotation.z : targetRotation.z;
                transform.rotation = Quaternion.LookRotation(targetRotation);
            }
                

        }

        private static void FetchPlayer()
        {
            if(_playerTransform == null) 
                _playerTransform = GameObject.FindGameObjectWithTag(TagsAndLayers.Tags.Player).transform;
        }
    }
}
