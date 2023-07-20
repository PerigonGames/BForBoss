using TMPro;
using UnityEngine;

namespace Perigon.UserInterface
{
    public class DropdownBehaviour : MonoBehaviour
    {
        private TMP_Dropdown _dropDown = null;

        public TMP_Dropdown.DropdownEvent onValueChanged => _dropDown.onValueChanged;

        public int value
        {
            get => _dropDown.value;
            set => _dropDown.value = value;
        }

        private void OnValidate()
        {
            _dropDown = GetComponentInChildren<TMP_Dropdown>();
            if (_dropDown == null)
            {
                Debug.LogWarning("TMP Dropdown missing from DropdownBehaviour");
            }
        }
    }
}
