using UnityEngine;

namespace BForBoss
{
    public class WallRunDataContainer : MonoBehaviour
    {
        [SerializeField] 
        private WallRunSO _wallRunSerializedObject;

        public WallRunData GetData => _wallRunSerializedObject.MapToData();
    }
}
