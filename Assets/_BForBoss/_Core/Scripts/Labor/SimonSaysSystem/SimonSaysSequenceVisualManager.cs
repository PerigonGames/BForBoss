using System;
using System.Collections;
using System.Collections.Generic;
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

        private IEnumerator coroutine;
        public event Action OnCompletedDisplaySequence;
        private void Awake()
        {
            this.PanicIfNullObject(_blueIndicator, nameof(_blueIndicator));
            this.PanicIfNullObject(_redIndicator, nameof(_redIndicator));
            this.PanicIfNullObject(_greenIndicator, nameof(_greenIndicator));
            this.PanicIfNullObject(_yellowIndicator, nameof(_yellowIndicator));
            this.PanicIfNullObject(_purpleIndicator, nameof(_purpleIndicator));
        }
        
        public void Initialize(Dictionary<SimonSaysColor, Color> colorMap)
        {
            foreach (var map in colorMap)
            {
                if (map.Key != SimonSaysColor.None)
                {
                    var indicator = MapToIndicator(map.Key);
                    indicator.Initialize(map.Value);
                }
            }
        }
        
        public void Reset()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }

            _blueIndicator.Reset();
            _redIndicator.Reset();
            _greenIndicator.Reset();
            _yellowIndicator.Reset();
            _purpleIndicator.Reset();
        }

        public void StartDisplaySequence(SimonSaysColorData[] dataArray)
        {
            coroutine = DisplaySequence(dataArray);
            StartCoroutine(coroutine);
        }

        private IEnumerator DisplaySequence(SimonSaysColorData[] dataArray)
        {
            foreach (var data in dataArray)
            {
                var indicator = MapToIndicator(data.SimonSaysColor);
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
