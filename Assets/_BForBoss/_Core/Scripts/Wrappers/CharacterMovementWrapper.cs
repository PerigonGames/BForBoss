using Perigon.Character;
using Perigon.Weapons;
using UnityEngine;

namespace BForBoss
{
    public class CharacterMovementWrapper : ICharacterMovement
    {
        private readonly FirstPersonPlayer _player = null;

        public Vector3 CharacterVelocity => _player.GetVelocity();
        public float CharacterMaxSpeed => _player.GetMaxSpeed();
        public bool IsGrounded => _player.IsOnGround();
        public bool IsSliding => _player.IsSliding();
        public bool IsDashing => _player.IsDashing();
        public bool IsWallRunning => _player.IsWallRunning();

        public CharacterMovementWrapper(FirstPersonPlayer player)
        {
            _player = player;
        }
    }
}
