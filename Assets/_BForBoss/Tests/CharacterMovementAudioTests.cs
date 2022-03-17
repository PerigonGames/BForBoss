using NUnit.Framework;
using Perigon.Character;
using UnityEngine;

namespace Tests.Character
{
    public class CharacterMovementAudioTests
    {
        private const float MIN_SPEED  = 5f;
        
        [Test]
        public void GetNextStateFromNotPlaying_whenSpeedMagnitudeHigherThanMinSpeed_whenIsWallRunningTrue_resultingStateWallRunning()
        {
            // Given
            var dummyCharacterMovement = new DummyCharacterMovement();
            var minSpeed = 1;
            var currentSpeed = 10;
            dummyCharacterMovement.IsWallRunning = true;
            dummyCharacterMovement.SpeedMagnitude = currentSpeed;
            var movementAudio = new CharacterMovementAudio(dummyCharacterMovement, minSpeed);
            MovementSoundState resultingState = MovementSoundState.NotPlaying;
            movementAudio.OnSoundStateChange += delegate(MovementSoundState state)
            {
                resultingState = state;
            };
                
            // When
            movementAudio.OnUpdate();
            
            // Then
            Assert.AreEqual(MovementSoundState.WallRunning, resultingState);
        }
        
        [Test]
        public void GetNextStateFromNotPlaying_whenSpeedMagnitudeLowerThanMinSpeed_resultingStateNotPlaying()
        {
            // Given
            var dummyCharacterMovement = new DummyCharacterMovement();
            var minSpeed = 10;
            var currentSpeed = 1;
            dummyCharacterMovement.IsWallRunning = true;
            dummyCharacterMovement.SpeedMagnitude = currentSpeed;
            var movementAudio = new CharacterMovementAudio(dummyCharacterMovement, minSpeed);
            MovementSoundState resultingState = MovementSoundState.NotPlaying;
            movementAudio.OnSoundStateChange += delegate(MovementSoundState state)
            {
                resultingState = state;
            };
                
            // When
            movementAudio.OnUpdate();
            
            // Then
            Assert.AreEqual(MovementSoundState.NotPlaying, resultingState);
        }
        
        [Test]
        public void GetNextStateFromNotPlaying_whenWalking_resultingStateRunning()
        {
            // Given
            var dummyCharacterMovement = new DummyCharacterMovement();
            var minSpeed = 1;
            var currentSpeed = 10;
            dummyCharacterMovement.IsWallRunning = false;
            dummyCharacterMovement._isWalking = true;
            dummyCharacterMovement.SpeedMagnitude = currentSpeed;
            var movementAudio = new CharacterMovementAudio(dummyCharacterMovement, minSpeed);
            MovementSoundState resultingState = MovementSoundState.NotPlaying;
            movementAudio.OnSoundStateChange += delegate(MovementSoundState state)
            {
                resultingState = state;
            };
                
            // When
            movementAudio.OnUpdate();
            
            // Then
            Assert.AreEqual(MovementSoundState.Running, resultingState);
        }
        
        [Test]
        public void GetNextStateFromNotPlaying_nothingPasses_resultingNotPlaying()
        {
            // Given
            var dummyCharacterMovement = new DummyCharacterMovement();
            var currentSpeed = 10;
            dummyCharacterMovement.IsWallRunning = false;
            dummyCharacterMovement._isWalking = false;
            dummyCharacterMovement.SpeedMagnitude = currentSpeed;
            var movementAudio = new CharacterMovementAudio(dummyCharacterMovement, MIN_SPEED);
            MovementSoundState resultingState = MovementSoundState.NotPlaying;
            movementAudio.OnSoundStateChange += delegate(MovementSoundState state)
            {
                resultingState = state;
            };
                
            // When
            movementAudio.OnUpdate();
            
            // Then
            Assert.AreEqual(MovementSoundState.NotPlaying, resultingState, "there isn't supposed to be a state change since new and old state is the same");
        }
        
        [Test]
        public void GetNextStateFromRunning_whenNotWalkingOrWallRunning_whenLowSpeed_thenNotPlaying()
        {
            // Given
            var dummyCharacterMovement = new DummyCharacterMovement();
            var movementAudio = GenerateCharacterMovementAudio(dummyCharacterMovement, MovementSoundState.Running);
            dummyCharacterMovement.IsWallRunning = false;
            dummyCharacterMovement._isWalking = false;
            dummyCharacterMovement.SpeedMagnitude = MIN_SPEED - 1;
            MovementSoundState resultingState = MovementSoundState.Running;
            movementAudio.OnSoundStateChange += delegate(MovementSoundState state)
            {
                resultingState = state;
            };
                
            // When
            movementAudio.OnUpdate();
            
            // Then
            Assert.AreEqual(MovementSoundState.NotPlaying, resultingState);
        }
        
        [Test]
        public void GetNextStateFromRunning_whenWallRunning_thenPlayWallRun()
        {
            // Given
            var dummyCharacterMovement = new DummyCharacterMovement();
            var movementAudio = GenerateCharacterMovementAudio(dummyCharacterMovement, MovementSoundState.Running);
            dummyCharacterMovement.IsWallRunning = true;
            dummyCharacterMovement._isWalking = false;
            dummyCharacterMovement.SpeedMagnitude = MIN_SPEED + 1;
            MovementSoundState resultingState = MovementSoundState.Running;
            movementAudio.OnSoundStateChange += delegate(MovementSoundState state)
            {
                resultingState = state;
            };
                
            // When
            movementAudio.OnUpdate();
            
            // Then
            Assert.AreEqual(MovementSoundState.WallRunning, resultingState);
        }
        
        [Test]
        public void GetNextStateFromRunning_whenNothingPasses_thenPlayRunning()
        {
            // Given
            var dummyCharacterMovement = new DummyCharacterMovement();
            var movementAudio = GenerateCharacterMovementAudio(dummyCharacterMovement, MovementSoundState.Running);
            dummyCharacterMovement.IsWallRunning = false;
            dummyCharacterMovement._isWalking = true;
            dummyCharacterMovement.SpeedMagnitude = MIN_SPEED + 1;
            MovementSoundState resultingState = MovementSoundState.Running;
            movementAudio.OnSoundStateChange += delegate(MovementSoundState state)
            {
                resultingState = state;
            };
                
            // When
            movementAudio.OnUpdate();
            
            // Then
            Assert.AreEqual(MovementSoundState.Running, resultingState, "there isn't supposed to be a state change, so it wil stay the same");
        }
        
        [Test]
        public void GetNextStateFromWallRunning_whenNotWallRunningOrWalking_thenNotPlaying()
        {
            // Given
            var dummyCharacterMovement = new DummyCharacterMovement();
            var movementAudio = GenerateCharacterMovementAudio(dummyCharacterMovement, MovementSoundState.WallRunning);
            dummyCharacterMovement.IsWallRunning = false;
            dummyCharacterMovement._isWalking = false;
            dummyCharacterMovement.SpeedMagnitude = MIN_SPEED + 1;
            MovementSoundState resultingState = MovementSoundState.Running;
            movementAudio.OnSoundStateChange += delegate(MovementSoundState state)
            {
                resultingState = state;
            };
                
            // When
            movementAudio.OnUpdate();
            
            // Then
            Assert.AreEqual(MovementSoundState.NotPlaying, resultingState);
        }
        
        [Test]
        public void GetNextStateFromWallRunning_whenNoSpeed_thenNotPlaying()
        {
            // Given
            var dummyCharacterMovement = new DummyCharacterMovement();
            var movementAudio = GenerateCharacterMovementAudio(dummyCharacterMovement, MovementSoundState.WallRunning);
            dummyCharacterMovement.IsWallRunning = true;
            dummyCharacterMovement._isWalking = true;
            dummyCharacterMovement.SpeedMagnitude = MIN_SPEED - 1;
            MovementSoundState resultingState = MovementSoundState.Running;
            movementAudio.OnSoundStateChange += delegate(MovementSoundState state)
            {
                resultingState = state;
            };
                
            // When
            movementAudio.OnUpdate();
            
            // Then
            Assert.AreEqual(MovementSoundState.NotPlaying, resultingState);
        }

        [Test]
        public void GetNextStateFromWallRunning_whenNotWallRunningWithSpeed_thenPlayRunning()
        {
            // Given
            var dummyCharacterMovement = new DummyCharacterMovement();
            var movementAudio = GenerateCharacterMovementAudio(dummyCharacterMovement, MovementSoundState.WallRunning);
            dummyCharacterMovement.IsWallRunning = false;
            dummyCharacterMovement._isWalking = true;
            dummyCharacterMovement.SpeedMagnitude = MIN_SPEED + 1;
            MovementSoundState resultingState = MovementSoundState.NotPlaying;
            movementAudio.OnSoundStateChange += delegate(MovementSoundState state)
            {
                resultingState = state;
            };
                
            // When
            movementAudio.OnUpdate();
            
            // Then
            Assert.AreEqual(MovementSoundState.Running, resultingState);
        }
        
        [Test]
        public void GetNextStateFromWallRunning_whenWallRunningWithSpeed_thenPlayWallRun()
        {
            // Given
            var dummyCharacterMovement = new DummyCharacterMovement();
            var movementAudio = GenerateCharacterMovementAudio(dummyCharacterMovement, MovementSoundState.WallRunning);
            dummyCharacterMovement.IsWallRunning = true;
            dummyCharacterMovement._isWalking = true;
            dummyCharacterMovement.SpeedMagnitude = MIN_SPEED + 1;
            MovementSoundState resultingState = MovementSoundState.NotPlaying;
            movementAudio.OnSoundStateChange += delegate(MovementSoundState state)
            {
                resultingState = state;
            };
                
            // When
            movementAudio.OnUpdate();
            
            // Then
            Assert.AreEqual(MovementSoundState.NotPlaying, resultingState, "There isn't supposed to be a state change since it's the same as past value");
        }

        private CharacterMovementAudio GenerateCharacterMovementAudio(DummyCharacterMovement movement, MovementSoundState state)
        {
            if (state == MovementSoundState.WallRunning)
            {
                movement.SpeedMagnitude = 10;
                movement.IsWallRunning = true;
            } 
            else if (state == MovementSoundState.Running)
            {
                movement.SpeedMagnitude = 10;
                movement.IsWallRunning = false;
                movement._isWalking = true;
            }

            var movementAudio = new CharacterMovementAudio(movement, MIN_SPEED);
            movementAudio.OnUpdate();
            return movementAudio;

        }
    }

    public class DummyCharacterMovement : ICharacterMovement
    {
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
