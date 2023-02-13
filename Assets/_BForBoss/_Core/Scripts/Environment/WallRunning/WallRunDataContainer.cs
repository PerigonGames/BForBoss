using Perigon.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BForBoss
{
    public class WallRunDataContainer : MonoBehaviour
    {
        [SerializeField, InlineEditor] 
        private WallRunSO _wallRunSerializedObject;

        public WallRunData GetData => _wallRunSerializedObject.MapToData();

        private void Awake()
        {
            gameObject.layer = TagsAndLayers.Layers.ParkourWallMask;
        }
    }
}
