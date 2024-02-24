using System;
using System.Collections;
using Perigon.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BForBoss
{
    public class SimonSaysSequenceVisualManager : MonoBehaviour
    {
        [Title("Configuration")] 
        [SerializeField] private float _showColorDuration = 1;
        [SerializeField] private float _waitTimeDuration = 1;
        
        [Title("Components")]
        [Resolve] [SerializeField] private SimonSaysIndicatorBehaviour _blueIndicator;
        [Resolve] [SerializeField] private SimonSaysIndicatorBehaviour _redIndicator;
        [Resolve] [SerializeField] private SimonSaysIndicatorBehaviour _greenIndicator;
        [Resolve] [SerializeField] private SimonSaysIndicatorBehaviour _yellowIndicator;
        [Resolve] [SerializeField] private SimonSaysIndicatorBehaviour _purpleIndicator;

        public event Action OnCompletedDisplaySequence;
        private void Awake()
        {
            this.PanicIfNullObject(_blueIndicator, nameof(_blueIndicator));
            this.PanicIfNullObject(_redIndicator, nameof(_redIndicator));
            this.PanicIfNullObject(_greenIndicator, nameof(_greenIndicator));
            this.PanicIfNullObject(_yellowIndicator, nameof(_yellowIndicator));
            this.PanicIfNullObject(_purpleIndicator, nameof(_purpleIndicator));
            
            _blueIndicator.Initialize(Color.blue);
            _redIndicator.Initialize(Color.red);
            _greenIndicator.Initialize(Color.green);
            _yellowIndicator.Initialize(Color.yellow);
            _purpleIndicator.Initialize(Color.magenta);
        }

        [Button]
        public void DebugShowSequence()
        {
            StartDisplaySequence(new[] { SimonSaysColor.Blue, SimonSaysColor.Purple, SimonSaysColor.Yellow });
        }

        public void StartDisplaySequence(SimonSaysColor[] colors)
        {
            StartCoroutine(DisplaySequence(colors));
        }

        private IEnumerator DisplaySequence(SimonSaysColor[] colors)
        {
            foreach (var color in colors)
            {
                var indicator = MapToIndicator(color);
                indicator.SetColor();
                yield return new WaitForSeconds(_showColorDuration);
                indicator.Reset();
                yield return new WaitForSeconds(_waitTimeDuration);
            }
            
            OnCompletedDisplaySequence?.Invoke();
        }

        private SimonSaysIndicatorBehaviour MapToIndicator(SimonSaysColor color)
        {
            switch (color)
            {
                case SimonSaysColor.Blue:
                    return _blueIndicator;
                case SimonSaysColor.Red:
                    return _redIndicator;
                case SimonSaysColor.Green:
                    return _greenIndicator;
                case SimonSaysColor.Yellow:
                    return _yellowIndicator;
                case SimonSaysColor.Purple:
                    return _purpleIndicator;
                default:
                    PanicHelper.Panic(new Exception("Missing Color from Simon Says Sequence Visual Manager"));
                    return null;
            }
        }
    }
}
