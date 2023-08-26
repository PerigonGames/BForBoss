using UnityEngine;
using UnityEngine.Pool;

namespace BForBoss
{
    public class IndicatorObjectPool : MonoBehaviour
    {
        [SerializeField] private IndicatorBehaviour _indicatorPrefab;
        private IObjectPool<IndicatorBehaviour> _pool = null;

        public IndicatorBehaviour GetIndicator()
        {
            return _pool.Get();
        }

        private void Awake()
        {
            _pool = new ObjectPool<IndicatorBehaviour>(
                createFunc: () =>
                {
                    var indicator = Instantiate(_indicatorPrefab);
                    indicator.Initialize(_pool);
                    indicator.transform.parent = transform.parent;
                    return indicator;
                },
                actionOnGet: (indicator) => indicator.gameObject.SetActive(true),
                actionOnRelease: (indicator) => indicator.gameObject.SetActive(false));
        }
    }
}
