using System;
using System.Collections.Generic;
using BForBoss.Labor;
using Perigon.Utility;
using Sirenix.OdinInspector;
using TMPro;
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
        FailedSequence,
        SucceededSequence,
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
        [SerializeField] private TMP_Text _resultLabel;

        private SimonSaysLabor _labor;
        private SimonSaysState _state = SimonSaysState.PrepareToStart;

        public event Action OnSucceededSimonSays;

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
            _labor.OnLaborCompleted += HandleOnLaborCompleted;
            _blocksManager.Initialize(_colorMap);
        }

        private void HandleOnLaborCompleted(ILabor sender, OnLaborCompletedArgs onCompletedArg)
        {
            if (onCompletedArg.DidSucceed)
            {
                OnSucceededSequence();
            }
            else
            {
                OnFailedSequence();
            }
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
            _resultLabel.text = string.Empty;
        }

        private void OnCompleteVisualSequence()
        {
            _state = SimonSaysState.ShowColoredIndicators;
            _blocksManager.SetBlockColors(_labor.SequenceOfColors);
            _state = SimonSaysState.PlayInProgress;
            _labor.Activate();
        }
        
        private void OnFailedSequence()
        {
            Debug.Log("Failed Sequence!");
            _state = SimonSaysState.FailedSequence;
            Reset();
            
            //Placeholder
            _resultLabel.text = "Failed!";
            _resultLabel.color = Color.red;
        }

        private void OnSucceededSequence()
        {
            Debug.Log("Succeeded Sequence!");
            _state = SimonSaysState.SucceededSequence;
            Reset();
            //TODO - Probably need animation before calling this at some point
            OnSucceededSimonSays?.Invoke();
            //Placeholder
            _resultLabel.text = "Succeeded!";
            _resultLabel.color = Color.green;
        }
    }
}
