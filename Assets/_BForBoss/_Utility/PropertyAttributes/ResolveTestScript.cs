using System;
using System.Collections.Generic;
using UnityEngine;

namespace BForBoss
{
    [Serializable]
    public struct ResolveStructExample
    {
        [Resolve] public AudioSource _AudioSource;
        [Resolve] [SerializeField] private Camera _camera;
    }
    
    public class ResolveTestScript : MonoBehaviour
    {
        //[Resolve] public int Integer = 3;
        //[Resolve] [SerializeField] private Collider _collider;

        //[Resolve] public List<BoxCollider> boxColliders;
        //[Resolve] public string[] array_strings;
        //[Resolve] [SerializeField] private List<int> _numbersTest;
        [Resolve][SerializeField] private BoxCollider[] _boxColliders_Array;
    }
}
