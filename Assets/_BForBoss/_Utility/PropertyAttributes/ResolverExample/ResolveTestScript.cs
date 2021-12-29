using System;
using System.Collections.Generic;
using UnityEngine;

namespace BForBoss
{
    [Serializable]
    public struct ResolveStructExample
    {
        [Resolve] [SerializeField] private Camera _camera;
        [Resolve] [SerializeField] private List<Camera> _camera_List;
        [Resolve] [SerializeField] private Camera[] _camera_Array;
    }
    
    public class ResolveTestScript : MonoBehaviour
    {
        [Resolve] public int Integer = 3;
        [Resolve] public Collider _collider;
        
        [Resolve, SerializeField] private List<Collider> _colliders_List;
        [Resolve, SerializeField] private Collider[] _colliders_Array;
        
        [SerializeField] private ResolveStructExample _struct;

        //[Resolve, SerializeField] public ResolveStructExample _structThatWillBreakImplementation;
    }
}
