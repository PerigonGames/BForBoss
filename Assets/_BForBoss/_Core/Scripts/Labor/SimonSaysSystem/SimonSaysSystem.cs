using System.Collections.Generic;
using Perigon.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BForBoss
{
    public enum SimonSaysColor
    {
        Blue,
        Red,
        Green,
        Yellow,
        Purple,
        None,
    }

    public enum SimonSaysState
    {
        PrepareToStart,
        DisplaySequence,
        ShowColoredIndicators,
        PlayInProgress,
    }
    
    public class SimonSaysSystem : SerializedMonoBehaviour
    {
        [Title("Configuration")]
        [SerializeField] private Dictionary<SimonSaysColor, Color> _colorMap;
        [SerializeField] private int _sequenceLength = 3;

        [Title("Components")]
        [SerializeField] private SimonSaysSequenceVisualManager _sequenceVisualManager;
        private SimonSaysState _state = SimonSaysState.PrepareToStart;
        private SimonSaysColor[] _sequence;

        private void Awake()
        {
            this.PanicIfNullObject(_colorMap, nameof(_colorMap));
        }

        [Button]
        public void Initialize()
        {
            _sequenceVisualManager.Initialize(_colorMap);
            _sequenceVisualManager.OnCompletedDisplaySequence += OnCompleteVisualSequence;
        }

        [Button]
        public void StartSystem()
        {
            _state = SimonSaysState.DisplaySequence;
            _sequence = BuildRandomSequence();
            _sequenceVisualManager.StartDisplaySequence(_sequence);
        }
        
        private SimonSaysColor[] BuildRandomSequence()
        {
            var sequence = new SimonSaysColor[_sequenceLength];
            for (int i = 0; i < sequence.Length; i++)
            {
                sequence[i] = (SimonSaysColor)UnityEngine.Random.Range(0, 5);
            }
            return sequence;
        }
        
        private void OnCompleteVisualSequence()
        {
            _state = SimonSaysState.ShowColoredIndicators;
        }


    }
}
