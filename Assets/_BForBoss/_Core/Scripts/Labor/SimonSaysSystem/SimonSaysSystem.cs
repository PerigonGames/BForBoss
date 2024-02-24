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
    
    public struct SimonSaysColorData
    {
        public SimonSaysColor SimonSaysColor;
        public Color Color;
        
        public SimonSaysColorData(SimonSaysColor simonSaysColor, Color color)
        {
            SimonSaysColor = simonSaysColor;
            Color = color;
        }
    }
    
    public class SimonSaysSystem : SerializedMonoBehaviour
    {
        [Title("Configuration")]
        [SerializeField] private Dictionary<SimonSaysColor, Color> _colorMap;
        [SerializeField] private int _sequenceLength = 3;

        [Title("Components")]
        [SerializeField] private SimonSaysSequenceVisualManager _sequenceVisualManager;
        [SerializeField] private SimonSaysBlocksManager _blocksManager;

        private SimonSaysLabor _labor;
        private SimonSaysState _state = SimonSaysState.PrepareToStart;

        private void Awake()
        {
            this.PanicIfNullObject(_colorMap, nameof(_colorMap));
            this.PanicIfNullObject(_sequenceVisualManager, nameof(_sequenceVisualManager));
            this.PanicIfNullObject(_blocksManager, nameof(_blocksManager));
        }

        public void Initialize()
        {
            _labor = new SimonSaysLabor(_blocksManager.Blocks, _colorMap, _sequenceLength);
            _sequenceVisualManager.Initialize(_colorMap);
            _sequenceVisualManager.OnCompletedDisplaySequence += OnCompleteVisualSequence;
            _blocksManager.Initialize(_colorMap);
        }

        public void StartSystem()
        {
            _state = SimonSaysState.DisplaySequence;
            _sequenceVisualManager.StartDisplaySequence(_labor.SequenceOfColors);
        }

        public void Reset()
        {
            _sequenceVisualManager.Reset();
            _labor.Reset();
            _blocksManager.Reset();
        }

        private void OnCompleteVisualSequence()
        {
            _state = SimonSaysState.ShowColoredIndicators;
            _blocksManager.SetBlockColors(_labor.SequenceOfColors);
            _state = SimonSaysState.PlayInProgress;
        }
    }
}
