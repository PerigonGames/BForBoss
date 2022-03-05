using Perigon.Entities;
using TMPro;
using UnityEngine;

namespace BForBoss
{
    public class EntityCounterViewBehaviour : MonoBehaviour
    {
        [SerializeField] private TMP_Text _entityCounterLabel = null;
        private int _entitiesDestroyed = 0;

        public void Initialize(LifeCycleManager lifeCycleManager)
        {
            lifeCycleManager.OnLivingEntitiesAmountChanged += HandleLivingEntitiesAmountChanged;
        }

        public void Reset()
        {
            _entitiesDestroyed = 0;
            _entityCounterLabel.text = _entitiesDestroyed.ToString();
        }

        private void HandleLivingEntitiesAmountChanged()
        {
            _entitiesDestroyed++;
            _entityCounterLabel.text = _entitiesDestroyed.ToString();
        }
        
    }
}
