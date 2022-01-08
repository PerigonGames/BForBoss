using System;
using System.Collections.Generic;
using UnityEngine;

namespace Perigon.Utility
{
    [Serializable]
    public struct ResolveStructExample
    {
        [Resolve] [SerializeField] private Camera _camera;
        [Resolve] [SerializeField] private List<Camera> _camera_List;
        [Resolve] [SerializeField] private Camera[] _camera_Array;
    }
    
    //[CreateAssetMenu(menuName = "Example/Resolver Tryout")]
    public class ResolverScriptableObject : ScriptableObject
    {
        public int finish = 3;
        [Resolve] public Collider _collider = null;
    }
    
    public class ResolveTestScript : MonoBehaviour
    {
         [Resolve] public int Integer = 3;
         [Resolve] public Collider _collider;
         
         [Resolve, SerializeField] private List<Collider> _colliders_List;
         [Resolve, SerializeField] private Collider[] _colliders_Array;
         
         [SerializeField] private ResolveStructExample _struct;
         
         [Resolve, SerializeField] private GameObject _go;
         
         [Resolve(IncludeInactive = false)] public ResolverScriptableObject _so = null;
        
        [Resolve, SerializeField] public ResolveStructExample _structThatWillBreakImplementation;

        //[Resolve, SerializeField] private List<ResolveStructExample> _listOfStructs;
    }
}
