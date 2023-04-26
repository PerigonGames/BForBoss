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
            public static int Player => LayerMask.NameToLayer("Player");
            public static int PlayerMask => ~(1 << Player);
            public static int  PlayerModel => 1 << LayerMask.NameToLayer("PlayerModel");
            public static int Enemy => LayerMask.GetMask("Enemy");
            public static LayerMask ParkourWallMask => LayerMask.GetMask("ParkourWall");
        }
    }
    
}
