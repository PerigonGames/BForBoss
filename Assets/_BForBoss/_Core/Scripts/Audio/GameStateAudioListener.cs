using FMODUnity;
using UnityEngine;

namespace BForBoss
{
    public class GameStateAudioListener : MonoBehaviour
    {
        [SerializeField] private EventReference _victorySFX;
        [SerializeField] private EventReference _failureSFX;
        [SerializeField] private EventReference _tutorialSFX;

        private void OnEnable()
        {
            StateManager.Instance.OnStateChanged += OnStateChanged;
        }
        
        private void OnDisable()
        {
            StateManager.Instance.OnStateChanged -= OnStateChanged;
        }

        private void OnStateChanged(State state)
        {
            Debug.Log("State changed: " + state);
            switch (state)
            {
                case State.Tutorial:
                    RuntimeManager.PlayOneShot(_tutorialSFX);
                    break;
                case State.Death:
                    RuntimeManager.PlayOneShot(_failureSFX);
                    break;
                case State.EndGame:
                    RuntimeManager.PlayOneShot(_victorySFX);
                    break;
            }
        }
    }
}
