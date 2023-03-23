using BForBoss;
using NUnit.Framework;

namespace Tests.Character
{
    public class CharacterMovementAudioTests
    {
        private const float MIN_SPEED  = 5f;
        
        [Test]
        public void GetNextStateFromNotPlaying_whenSpeedMagnitudeHigherThanMinSpeed_whenIsWallRunningTrue_resultingStateWallRunning()
        {
            // Given
            var dummyCharacterMovement = new MockCharacterMovement();
            var minSpeed = 1;
            var currentSpeed = 10;
            dummyCharacterMovement.IsWallRunning = true;
            dummyCharacterMovement._isLowerThanSpeed = false;
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
            var dummyCharacterMovement = new MockCharacterMovement();
            var minSpeed = 10;
            var currentSpeed = 1;
            dummyCharacterMovement.IsWallRunning = true;
            dummyCharacterMovement._isLowerThanSpeed = true;
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
        public void GetNextStateFromNotPlaying_whenRunning_resultingStateRunning()
        {
            // Given
            var dummyCharacterMovement = new MockCharacterMovement();
            var minSpeed = 1;
            var currentSpeed = 10;
            dummyCharacterMovement.IsWallRunning = false;
            dummyCharacterMovement.IsRunning = true;
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
        public void GetNextStateFromNotPlaying_NoMovement_resultingNotPlaying()
        {
            // Given
            var dummyCharacterMovement = new MockCharacterMovement();
            var currentSpeed = 10;
            dummyCharacterMovement.IsWallRunning = false;
            dummyCharacterMovement.IsRunning = false;
            dummyCharacterMovement._isLowerThanSpeed = true;
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
            var dummyCharacterMovement = new MockCharacterMovement();
            var movementAudio = GenerateCharacterMovementAudio(dummyCharacterMovement, MovementSoundState.Running);
            dummyCharacterMovement.IsWallRunning = false;
            dummyCharacterMovement.IsRunning = false;
            dummyCharacterMovement._isLowerThanSpeed = true;
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
            var dummyCharacterMovement = new MockCharacterMovement();
            var movementAudio = GenerateCharacterMovementAudio(dummyCharacterMovement, MovementSoundState.Running);
            dummyCharacterMovement.IsWallRunning = true;
            dummyCharacterMovement.IsRunning = false;
            dummyCharacterMovement._isLowerThanSpeed = false;
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
            var dummyCharacterMovement = new MockCharacterMovement();
            var movementAudio = GenerateCharacterMovementAudio(dummyCharacterMovement, MovementSoundState.Running);
            dummyCharacterMovement.IsWallRunning = false;
            dummyCharacterMovement.IsRunning = true;
            dummyCharacterMovement._isLowerThanSpeed = false;
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
            var dummyCharacterMovement = new MockCharacterMovement();
            var movementAudio = GenerateCharacterMovementAudio(dummyCharacterMovement, MovementSoundState.WallRunning);
            dummyCharacterMovement.IsWallRunning = false;
            dummyCharacterMovement.IsRunning = false;
            dummyCharacterMovement._isLowerThanSpeed = false;
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
            var dummyCharacterMovement = new MockCharacterMovement();
            var movementAudio = GenerateCharacterMovementAudio(dummyCharacterMovement, MovementSoundState.WallRunning);
            dummyCharacterMovement.IsWallRunning = true;
            dummyCharacterMovement.IsRunning = true;
            dummyCharacterMovement._isLowerThanSpeed = true;
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
            var dummyCharacterMovement = new MockCharacterMovement();
            var movementAudio = GenerateCharacterMovementAudio(dummyCharacterMovement, MovementSoundState.WallRunning);
            dummyCharacterMovement.IsWallRunning = false;
            dummyCharacterMovement.IsRunning = true;
            dummyCharacterMovement._isLowerThanSpeed = false;
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
            var dummyCharacterMovement = new MockCharacterMovement();
            var movementAudio = GenerateCharacterMovementAudio(dummyCharacterMovement, MovementSoundState.WallRunning);
            dummyCharacterMovement.IsWallRunning = true;
            dummyCharacterMovement.IsRunning = true;
            dummyCharacterMovement._isLowerThanSpeed = false;
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

        private CharacterMovementAudio GenerateCharacterMovementAudio(MockCharacterMovement movement, MovementSoundState state)
        {
            if (state == MovementSoundState.WallRunning)
            {
                movement.IsWallRunning = true;
            } 
            else if (state == MovementSoundState.Running)
            {
                movement.IsWallRunning = false;
                movement.IsRunning = true;
            }

            var movementAudio = new CharacterMovementAudio(movement, MIN_SPEED);
            movementAudio.OnUpdate();
            return movementAudio;

        }
    }
}
