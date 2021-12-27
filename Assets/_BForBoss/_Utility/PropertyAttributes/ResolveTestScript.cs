using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BForBoss
{
    public class ResolveTestScript : MonoBehaviour
    {
        [Resolve] public int value = 3;
        [Resolve] [SerializeField] private BoxCollider collider;
    }
}
