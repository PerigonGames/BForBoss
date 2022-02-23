using UnityEngine;
using FMODUnity;

namespace Perigon.Character
{
    public class CharacterMovementAudio : MonoBehaviour
    {
        [SerializeField] private EventReference _dashAudio;
        [SerializeField] private EventReference _slideAudio;
        [SerializeField] private EventReference _jumpAudio;

        [SerializeField] private EventReference _runningAudio;
        [SerializeField] private EventReference _wallrunAudio;
    }
}
