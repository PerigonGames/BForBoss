using System.Collections;
using BForBoss;
using NUnit.Framework;
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
            EditorSceneManager.LoadSceneAsyncInPlayMode("Assets/_BForBoss/Tests/Scenes/CharacterMovementTest.unity", new LoadSceneParameters(LoadSceneMode.Single));
            _keyboard = InputSystem.AddDevice<Keyboard>();
        }

        [UnityTest]
        public IEnumerator Character_MoveForward_GreaterZPosition()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            var originalPosition = Vector3.zero;
            var character = GameObject.FindObjectOfType<FirstPersonPlayer>();
            character.transform.position = originalPosition;
            Press(_keyboard.wKey);
            
            yield return new WaitForSeconds(1.5f);

            Assert.Greater(character.transform.position.z, originalPosition.z, "Character walked forward, should be higher z value");
        }
        
        [UnityTest]
        public IEnumerator Character_MoveBackwards_LesserZPosition()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            var originalPosition = Vector3.zero;
            var character = GameObject.FindObjectOfType<FirstPersonPlayer>();
            character.transform.position = originalPosition;
            Press(_keyboard.sKey);
            
            yield return new WaitForSeconds(1.5f);

            Assert.Less(character.transform.position.z, originalPosition.z, "Character walked backwards, should be lower z value");
        }
        
        [UnityTest]
        public IEnumerator Character_MoveLeft_LesserXPosition()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            var originalPosition = Vector3.zero;
            var character = GameObject.FindObjectOfType<FirstPersonPlayer>();
            character.transform.position = originalPosition;
            Press(_keyboard.aKey);
            
            yield return new WaitForSeconds(1.5f);

            Assert.Less(character.transform.position.x, originalPosition.x, "Character walked to the left, should be lower x value");
        }
        
        [UnityTest]
        public IEnumerator Character_MoveRight_GreaterXPosition()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            var originalPosition = Vector3.zero;
            var character = GameObject.FindObjectOfType<FirstPersonPlayer>();
            character.transform.position = originalPosition;
            Press(_keyboard.dKey);
            
            yield return new WaitForSeconds(1.5f);

            Assert.Greater(character.transform.position.x, originalPosition.x, "Character walked to the right, should be higher x value");
        }
        
        [UnityTest]
        public IEnumerator Character_Jump_GreaterYPosition()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            var character = GameObject.FindObjectOfType<FirstPersonPlayer>();
            character.transform.position = Vector3.zero;
            
            // Wait for character to settle after repositioning to 0,0,0
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            var startingPosition = character.transform.position;
            Press(_keyboard.spaceKey);
            yield return new WaitForSeconds(0.5f);
            
            Assert.Greater(character.transform.position.y, startingPosition.y, "Character walked to the right, should be higher x value");
        }
    }
}