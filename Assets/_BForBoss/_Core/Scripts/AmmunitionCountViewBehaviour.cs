using Perigon.Weapons;
using TMPro;
using UnityEngine;

namespace BForBoss
{
    public class AmmunitionCountViewBehaviour : MonoBehaviour
    {
        private const string DEFAULT_AMMUNITION_TEXT = "0 / 0";
        private const string DEFAULT_WEAPON_NAME = "Pistol";
        [SerializeField] private TMP_Text _ammunitionCountLabel = null;
        [SerializeField] private TMP_Text _weaponNameLabel = null;

        private IEquipmentData _equipmentData = null;

        public void Initialize(IEquipmentData equipmentData)
        {
            _equipmentData = equipmentData;
        }

        public void Reset()
        {
            _ammunitionCountLabel.text = DEFAULT_AMMUNITION_TEXT;
            _weaponNameLabel.text = DEFAULT_WEAPON_NAME;
        }
        
        private void Update()
        {
            if (_equipmentData != null)
            {
                _ammunitionCountLabel.text = $"{_equipmentData?.AmmunitionAmount} / {_equipmentData?.MaxAmmunitionAmount}";
                _weaponNameLabel.text = _equipmentData?.NameOfWeapon;
            }
        }

        private void Awake()
        {
            _ammunitionCountLabel = GetComponentInChildren<TMP_Text>();
        }
    }
}
