using System;
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
    public class CharacterDashTests : InputTestFixture
    {
        private Vector3 Section_B_Location = new Vector3(0, 0, -40f);
        private Keyboard _keyboard = null;
        

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            EditorSceneManager.LoadSceneAsyncInPlayMode("Assets/_BForBoss/Tests/Scenes/CharacterDashTest.unity", new LoadSceneParameters(LoadSceneMode.Single));
            _keyboard = InputSystem.AddDevice<Keyboard>();
        }
        
        [UnityTest]
        public IEnumerator Character_Dash_StayingStill()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForFixedUpdate();
            }
            
            var originalPosition = Vector3.zero;
            var character = GameObject.FindObjectOfType<FirstPersonPlayer>();
            character.transform.position = originalPosition;
            var expectedHeight = character.transform.position.y;
            
            Press(_keyboard.leftShiftKey);
            yield return new WaitForSeconds(1.0f); 
            
            var withinBounds = Math.Abs(expectedHeight - character.transform.position.y) < 0.1f;
            Assert.IsTrue(withinBounds, "The height should have stayed the same after dashing over the hole");
            Assert.Greater(character.transform.position.z, originalPosition.z, "Character should have Dashed forward in Positive Z direction");
        }
        
        [UnityTest]
        public IEnumerator Character_Dash_PressingForward()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForFixedUpdate();
            }
            
            var originalPosition = Vector3.zero;
            var character = GameObject.FindObjectOfType<FirstPersonPlayer>();
            character.transform.position = originalPosition;
            var expectedHeight = character.transform.position.y;
            
            Press(_keyboard.wKey);
            yield return new WaitForSeconds(0);
            Press(_keyboard.leftShiftKey);
            yield return new WaitForSeconds(1.0f);
            
            var withinBounds = Math.Abs(expectedHeight - character.transform.position.y) < 0.1f;
            Assert.IsTrue(withinBounds, "The height should have stayed the same after dashing over the hole");
            Assert.Greater(character.transform.position.z, originalPosition.z, "Character should have Dashed forward in Positive Z direction");
        }
        
        [UnityTest]
        public IEnumerator Character_Dash_PressingBack()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForFixedUpdate();
            }
            
            var originalPosition = Vector3.zero;
            var character = GameObject.FindObjectOfType<FirstPersonPlayer>();
            character.transform.position = originalPosition;
            var expectedHeight = character.transform.position.y;
            
            Press(_keyboard.sKey);
            yield return new WaitForSeconds(0);
            Press(_keyboard.leftShiftKey);
            yield return new WaitForSeconds(1.0f);

            var withinBounds = Math.Abs(expectedHeight - character.transform.position.y) < 0.1f;
            Assert.IsTrue(withinBounds, "The height should have stayed the same after dashing over the hole");
            Assert.Less(character.transform.position.z, originalPosition.z, "Character should have Dashed backwards in Negative Z direction");
        }
        
        [UnityTest]
        public IEnumerator Character_Dash_PressingLeft()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForFixedUpdate();
            }
            
            var originalPosition = Vector3.zero;
            var character = GameObject.FindObjectOfType<FirstPersonPlayer>();
            character.transform.position = originalPosition;
            var expectedHeight = character.transform.position.y;
            
            Press(_keyboard.aKey);
            yield return new WaitForSeconds(0);
            Press(_keyboard.leftShiftKey);
            yield return new WaitForSeconds(1.0f);

            var withinBounds = Math.Abs(expectedHeight - character.transform.position.y) < 0.1f;
            Assert.IsTrue(withinBounds, "The height should have stayed the same after dashing over the hole");
            Assert.Less(character.transform.position.x, originalPosition.x, "Character should have Dashed Left in Negative X direction");
        }
        
        [UnityTest]
        public IEnumerator Character_Dash_PressingRight()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForFixedUpdate();
            }
            
            var originalPosition = Vector3.zero;
            var character = GameObject.FindObjectOfType<FirstPersonPlayer>();
            character.transform.position = originalPosition;
            var expectedHeight = character.transform.position.y;
            
            Press(_keyboard.dKey);
            yield return new WaitForSeconds(0);
            Press(_keyboard.leftShiftKey);
            yield return new WaitForSeconds(1.0f);

            var withinBounds = Math.Abs(expectedHeight - character.transform.position.y) < 0.1f;
            Assert.IsTrue(withinBounds, "The height should have stayed the same after dashing over the hole");
            Assert.Greater(character.transform.position.x, originalPosition.x, "Character should have Dashed Right in Positive X direction");
        }
        
        [UnityTest]
        public IEnumerator Character_Dash_WhileOneJump()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            var originalPosition = Section_B_Location;
            var character = GameObject.FindObjectOfType<FirstPersonPlayer>();
            character.transform.position = originalPosition;
            
            Press(_keyboard.wKey);
            Press(_keyboard.spaceKey);
            yield return new WaitForSeconds(0.5f);
            Press(_keyboard.leftShiftKey);
            yield return new WaitForSeconds(1.0f);

            Assert.Greater(character.transform.position.y, originalPosition.y, "The jump should have made the character Higher");
            Assert.Greater(character.transform.position.z, originalPosition.z, "Character should have Dashed Forward in Positive Z direction");
        }
        
        [UnityTest]
        public IEnumerator Character_Dash_WhileDoubleJump()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            var originalPosition = Section_B_Location;
            var character = GameObject.FindObjectOfType<FirstPersonPlayer>();
            character.transform.position = originalPosition;
            
            Press(_keyboard.spaceKey);
            yield return new WaitForSeconds(0.2f);
            Release(_keyboard.spaceKey);
            Press(_keyboard.spaceKey);
            yield return new WaitForSeconds(0.2f);
            Press(_keyboard.sKey);
            yield return new WaitForSeconds(0.1f);
            Press(_keyboard.leftShiftKey);
            yield return new WaitForSeconds(1.0f);

            Assert.Greater(character.transform.position.y, originalPosition.y, "The jump should have made the character Higher");
            Assert.Less(character.transform.position.z, originalPosition.z, "Character should have Dashed Backward in Negative Z direction");
        }
    }
}
