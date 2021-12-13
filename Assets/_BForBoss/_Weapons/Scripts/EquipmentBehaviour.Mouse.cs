using UnityEngine.InputSystem;

namespace Perigon.Weapons
{
    public partial class EquipmentBehaviour
    {
        private void SwapWeaponOnMouse()
        {
            var scrollVector = Mouse.current.scroll.ReadValue().normalized;
            if (scrollVector.y > 0)
            {
                _weapons[_currentWeaponIndex].enabled = false;
                UpdateCurrentWeaponIndex(true);
                _weapons[_currentWeaponIndex].enabled = true;
            }
            else if (scrollVector.y < 0)
            {
                _weapons[_currentWeaponIndex].enabled = false;
                UpdateCurrentWeaponIndex(false);
                _weapons[_currentWeaponIndex].enabled = true;
            }
        }

        private void UpdateCurrentWeaponIndex(bool isUpwards)
        {
            var indexLength = _weapons.Length - 1;
            _currentWeaponIndex += isUpwards ? 1 : -1;
            
            if (_currentWeaponIndex > indexLength)
            {
                _currentWeaponIndex = 0;
            } 
            else if (_currentWeaponIndex < 0)
            {
                _currentWeaponIndex = indexLength;
            }
        }
    }
}
