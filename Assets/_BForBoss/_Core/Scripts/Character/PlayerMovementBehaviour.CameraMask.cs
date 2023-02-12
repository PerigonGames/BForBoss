using Perigon.Utility;

namespace Perigon.Character
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
