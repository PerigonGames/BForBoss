using Perigon.Character;
using Perigon.Weapons;
using UnityEngine;

namespace BForBoss
{
    public class CharacterMovementWrapper : ICharacterMovement
    {
        private readonly FirstPersonPlayer _player = null;

        Vector3 ICharacterMovement.CharacterVelocity => _player.GetVelocity();
        float ICharacterMovement.CharacterMaxSpeed => _player.GetMaxSpeed();
        bool ICharacterMovement.IsGrounded => _player.IsOnGround();
        bool ICharacterMovement.IsSliding => _player.IsSliding();
        bool ICharacterMovement.IsDashing => _player.IsDashing();
        bool ICharacterMovement.IsWallRunning => _player.IsWallRunning();

        public CharacterMovementWrapper(FirstPersonPlayer player)
        {
            _player = player;
        }
    }
}
