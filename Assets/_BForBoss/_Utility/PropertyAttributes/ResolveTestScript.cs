using System.Collections.Generic;
using UnityEngine;

namespace BForBoss
{
    public class ResolveTestScript : MonoBehaviour
    {
        //[Resolve] public int Integer = 3;
        [Resolve] [SerializeField] private BoxCollider BoxCollider;

        //[Resolve] public List<BoxCollider> boxColliders;
        //[Resolve] public string[] array_strings;
        //[Resolve] [SerializeField] private List<int> _numbersTest;
        [Resolve] [SerializeField] private BoxCollider[] _boxColliders_Array;
    }
}
