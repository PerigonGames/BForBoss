using Perigon.Entities;
using TMPro;
using UnityEngine;

namespace BForBoss
{
    public class EntityCounterViewBehaviour : MonoBehaviour
    {
        [SerializeField] private TMP_Text _entityCounterLabel = null;
        private LifeCycleManager _lifeCycleManager = null;

        public void Initialize(LifeCycleManager lifeCycleManager)
        {
            _lifeCycleManager = lifeCycleManager;
            _lifeCycleManager.OnLivingEntityEliminated += HandleLivingEntitiesAmountChanged;
        }

        public void Reset()
        {
            _entityCounterLabel.text = "0";
        }

        private void HandleLivingEntitiesAmountChanged(int amount)
        {
            _entityCounterLabel.text = amount.ToString();
        }

        private void OnDestroy()
        {
            _lifeCycleManager.OnLivingEntityEliminated -= HandleLivingEntitiesAmountChanged;
        }
    }
}
