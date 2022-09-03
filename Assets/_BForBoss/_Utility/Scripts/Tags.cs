using UnityEngine;

namespace Perigon.Utility
{
    public static class TagsAndLayers 
    {
        public static class Tags
        {
            public static string Player => "Player";
        }

        public static class Layers
        {
            public static int TriggerArea => 1 << LayerMask.NameToLayer("TriggerArea");
            public static int PlayerMask => ~LayerMask.GetMask("Player");
            public static int  PlayerModel => 1 << LayerMask.NameToLayer("PlayerModel");
        }
    }
    
}
