using UnityEngine;

namespace Perigon.Character
{
    public partial class PlayerMovementBehaviour
    {
        private const string PLAYER_MODEL_LAYER = "PlayerModel";
        private const string FIRST_PERSON_WEAPON_LAYER = "FirstPersonWeapon";
        
        private int FirstPersonMask => ~(1 << LayerMask.NameToLayer(PLAYER_MODEL_LAYER) | 1 << LayerMask.NameToLayer(FIRST_PERSON_WEAPON_LAYER));

        private void SetCameraCullingMask()
        {
            camera.cullingMask = FirstPersonMask;
        }
    }
}
