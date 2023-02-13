using Perigon.Utility;

namespace BForBoss
{
    public partial class PlayerMovementBehaviour
    {
        private int FirstPersonMask => ~TagsAndLayers.Layers.PlayerModel;

        private void SetCameraCullingMask()
        {
            camera.cullingMask = FirstPersonMask;
        }
    }
}
