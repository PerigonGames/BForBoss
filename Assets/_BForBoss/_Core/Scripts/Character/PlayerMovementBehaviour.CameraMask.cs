using Perigon.Utility;

namespace BForBoss
{
    public partial class PlayerMovementBehaviour
    {
        private void SetCameraCullingMask()
        {
            camera.cullingMask = ~TagsAndLayers.Mask.PlayerModelMask;
        }
    }
}
