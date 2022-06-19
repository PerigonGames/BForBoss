using Perigon.Character;
using UnityEngine;

namespace Tests
{
    public class MockCharacterMovement : ICharacterMovement
    {
        public Transform RootPivot { get; set; }
        public bool _isWalking = false;
        public Vector3 CharacterVelocity { get; set; }
        public float CharacterMaxSpeed { get; set; }
        public bool IsGrounded { get; set; }
        public bool IsDashing { get; set; }
        public bool IsSliding { get; set; }
        public bool IsWallRunning { get; set; }
        public float SpeedMagnitude { get; set; }
        public bool IsWalking()
        {
            return _isWalking;
        }
    }
}
