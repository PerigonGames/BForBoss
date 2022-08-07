using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perigon.Utility
{
    public class ChildToCamera : MonoBehaviour
    {
        void Start()
        {
            var mainCam = Camera.main;
            if(mainCam != null)
                transform.parent = mainCam.transform;
        }
    }
}
