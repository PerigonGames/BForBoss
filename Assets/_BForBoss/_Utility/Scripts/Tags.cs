using UnityEngine;

namespace Perigon.Utility
{
    public static class TagsAndLayers 
    {
        public static class Tags
        {
            public static string Player => "Player";
        }

        /*
         * Used for layer comparison
         */
        public static class Layers
        {
            public static int Player => LayerMask.NameToLayer("Player");
            public static int PlayerModel => LayerMask.NameToLayer("PlayerModel");
            public static int Enemy => LayerMask.NameToLayer("Enemy");
            public static int ParkourWall => LayerMask.NameToLayer("ParkourWall");

            public static int HookshotTarget => LayerMask.NameToLayer("HookshotTarget");
        }

        /*
         * Used for Raycast and physics Overlap
         */
        public static class Mask
        {
            public static int PlayerMask => 1 << Layers.Player;
            public static int PlayerModelMask => 1 << Layers.PlayerModel;
            public static int EnemyMask => 1 << Layers.Enemy;
            public static int ParkourWallMask => 1 << Layers.ParkourWall;
            public static int HookshotTargetMask => 1 << Layers.HookshotTarget;
        }
    }
    
}
