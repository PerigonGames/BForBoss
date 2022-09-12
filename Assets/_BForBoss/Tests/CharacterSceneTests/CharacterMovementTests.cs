using System.Collections;
using NUnit.Framework;
using Perigon.Character;
using Perigon.Utility;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests.Character
{
    public class CharacterMovementTests: InputTestFixture
    {
        private Keyboard _keyboard = null;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            EditorSceneManager.LoadSceneAsyncInPlayMode("Assets/_BForBoss/Tests/Scenes/GenericCharacterTests.unity", new LoadSceneParameters(LoadSceneMode.Single));
            _keyboard = InputSystem.AddDevice<Keyboard>();
        }

        [UnityTest]
        public IEnumerator Character_MoveForward_GreaterZPosition()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            var startingPosition = GameObject.Find("MovementSpawn").transform.position;
            var character = GameObject.FindObjectOfType<PlayerMovementBehaviour>(); 
            var mockWorld = GameObject.FindObjectOfType<MockGenericCharacterWorldManager>(); 
            var pgInputSystem = new PGInputSystem(mockWorld.ActionAsset);
            character.Initialize(pgInputSystem);
            pgInputSystem.SetToPlayerControls();
            character.transform.position = startingPosition;
            
            Press(_keyboard.wKey);
            
            yield return new WaitForSeconds(1.5f);
            
            Assert.Greater(character.transform.position.z, startingPosition.z, "Character walked forward, should be higher z value");
            pgInputSystem.ForceUnbind();
        }
        
        [UnityTest]
        public IEnumerator Character_MoveBackwards_LesserZPosition()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            var originalPosition = GameObject.Find("MovementSpawn").transform.position;
            var character = GameObject.FindObjectOfType<PlayerMovementBehaviour>();
            var mockWorld = GameObject.FindObjectOfType<MockGenericCharacterWorldManager>(); 
            var pgInputSystem = new PGInputSystem(mockWorld.ActionAsset);
            character.Initialize(pgInputSystem);
            pgInputSystem.SetToPlayerControls();
            character.transform.position = originalPosition;
            Press(_keyboard.sKey);
            
            yield return new WaitForSeconds(1.5f);

            Assert.Less(character.transform.position.z, originalPosition.z, "Character walked backwards, should be lower z value");
            pgInputSystem.ForceUnbind();
        }
        
        [UnityTest]
        public IEnumerator Character_MoveLeft_LesserXPosition()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            var originalPosition = GameObject.Find("MovementSpawn").transform.position;
            var character = GameObject.FindObjectOfType<PlayerMovementBehaviour>();
            var mockWorld = GameObject.FindObjectOfType<MockGenericCharacterWorldManager>(); 
            var pgInputSystem = new PGInputSystem(mockWorld.ActionAsset);
            character.Initialize(pgInputSystem);
            pgInputSystem.SetToPlayerControls();
            character.transform.position = originalPosition;
            Press(_keyboard.aKey);
            
            yield return new WaitForSeconds(1.5f);

            Assert.Less(character.transform.position.x, originalPosition.x, "Character walked to the left, should be lower x value");
            pgInputSystem.ForceUnbind();
        }
        
        [UnityTest]
        public IEnumerator Character_MoveRight_GreaterXPosition()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            var originalPosition = GameObject.Find("MovementSpawn").transform.position;
            var character = GameObject.FindObjectOfType<PlayerMovementBehaviour>();
            var mockWorld = GameObject.FindObjectOfType<MockGenericCharacterWorldManager>(); 
            var pgInputSystem = new PGInputSystem(mockWorld.ActionAsset);
            character.Initialize(pgInputSystem);
            pgInputSystem.SetToPlayerControls();
            character.transform.position = originalPosition;
            Press(_keyboard.dKey);
            
            yield return new WaitForSeconds(1.5f);

            Assert.Greater(character.transform.position.x, originalPosition.x, "Character walked to the right, should be higher x value");
            pgInputSystem.ForceUnbind();
        }
        
        [UnityTest]
        public IEnumerator Character_Jump_GreaterYPosition()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            var character = GameObject.FindObjectOfType<PlayerMovementBehaviour>();
            character.transform.position = GameObject.Find("MovementSpawn").transform.position;
            
            // Wait for character to settle after repositioning to 0,0,0
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            var startingPosition = character.transform.position;
            Press(_keyboard.spaceKey);
            yield return new WaitForSeconds(0.5f);
            
            Assert.Greater(character.transform.position.y, startingPosition.y, "Character jumped, y value should be higher");
        }
    }
}