using Perigon.Character;
using UnityEngine;
using UnityEngine.UI;

namespace BForBoss
{
    public class PlayerChargingViewBehaviour : MonoBehaviour
    {
        [SerializeField] private Image _dynamicBar;
        [SerializeField] private PlayerChargingSystemBehaviour _chargingSystem;

        private void Update()
        {
            _dynamicBar.fillAmount = _chargingSystem.Percentage;
        }
    }
}
