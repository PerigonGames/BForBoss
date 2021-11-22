using NUnit.Framework;
using System.Collections;
using Perigon.Character;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests.Character
{
    public class CharacterThirdPersonSlideTests : InputTestFixture
    {
        private Keyboard _keyboard = null;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            EditorSceneManager.LoadSceneAsyncInPlayMode("Assets/_BForBoss/Tests/Scenes/CharacterSlideTest.unity", new LoadSceneParameters(LoadSceneMode.Single));
            _keyboard = InputSystem.AddDevice<Keyboard>();
        }

        [UnityTest]
        public IEnumerator CharacterThirdPerson_CrouchLeft_BeyondLeftBlockade()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            var originalPosition = Vector3.zero;
            var character = GameObject.FindObjectOfType<FirstPersonPlayer>();
            var blockadeLeft = GameObject.Find("Blockade_Left");
            character.transform.position = originalPosition;

            Press(_keyboard.qKey);
            yield return new WaitForSeconds(0.25f);
            Press(_keyboard.cKey);
            yield return new WaitForSeconds(0.5f);
            Press(_keyboard.aKey);
            yield return new WaitForSeconds(2f);


            Assert.Less(character.transform.position.x, blockadeLeft.transform.position.x, "Crouching left should have went under and beyond the left blockade");
        }

        [UnityTest]
        public IEnumerator CharacterThirdPerson_CrouchRight_BeyondRightBlockade()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            var originalPosition = Vector3.zero;
            var character = GameObject.FindObjectOfType<FirstPersonPlayer>();
            var blockadeLeft = GameObject.Find("Blockade_Right");
            character.transform.position = originalPosition;

            Press(_keyboard.qKey);
            yield return new WaitForSeconds(0.25f);
            Press(_keyboard.cKey);
            yield return new WaitForSeconds(0.5f);
            Press(_keyboard.dKey);
            yield return new WaitForSeconds(2f);


            Assert.Greater(character.transform.position.x, blockadeLeft.transform.position.x, "Crouching right should have went under and beyond the right blockade");
        }

        [UnityTest]
        public IEnumerator CharacterThirdPerson_CrouchForward_BeyondForwardBlockade()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            var originalPosition = Vector3.zero;
            var character = GameObject.FindObjectOfType<FirstPersonPlayer>();
            var blockadeForward = GameObject.Find("Blockade_Forward");
            character.transform.position = originalPosition;

            Press(_keyboard.qKey);
            yield return new WaitForSeconds(0.25f);
            Press(_keyboard.cKey);
            yield return new WaitForSeconds(0.5f);
            Press(_keyboard.wKey);
            yield return new WaitForSeconds(2);


            Assert.Greater(character.transform.position.z, blockadeForward.transform.position.z, "Crouching forward should have went under and beyond the forward blockade");
        }

        [UnityTest]
        public IEnumerator CharacterThirdPerson_CrouchBackward_BeyondBackwardBlockade()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            var originalPosition = Vector3.zero;
            var character = GameObject.FindObjectOfType<FirstPersonPlayer>();
            var blockadeForward = GameObject.Find("Blockade_Backward");
            character.transform.position = originalPosition;

            Press(_keyboard.qKey);
            yield return new WaitForSeconds(0.25f);
            Press(_keyboard.cKey);
            yield return new WaitForSeconds(0.5f);
            Press(_keyboard.sKey);
            yield return new WaitForSeconds(2);


            Assert.Less(character.transform.position.z, blockadeForward.transform.position.z, "Crouch backward should have went under and beyond the backward blockade");
        }

        [UnityTest]
        public IEnumerator CharacterThirdPerson_SlideLeft_BeyondLeftBlockade()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            var originalPosition = Vector3.zero;
            var character = GameObject.FindObjectOfType<FirstPersonPlayer>();
            var blockadeLeft = GameObject.Find("Blockade_Left");
            character.transform.position = originalPosition;

            Press(_keyboard.qKey);
            yield return new WaitForSeconds(0.25f);
            Press(_keyboard.aKey);
            yield return new WaitForSeconds(0);
            Press(_keyboard.cKey);
            yield return new WaitForSeconds(1.0f);


            Assert.Less(character.transform.position.x, blockadeLeft.transform.position.x, "Sliding left should have went under and beyond the left blockade");
        }

        [UnityTest]
        public IEnumerator CharacterThirdPerson_SlideRight_BeyondRightBlockade()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            var originalPosition = Vector3.zero;
            var character = GameObject.FindObjectOfType<FirstPersonPlayer>();
            var blockadeLeft = GameObject.Find("Blockade_Right");
            character.transform.position = originalPosition;

            Press(_keyboard.qKey);
            yield return new WaitForSeconds(0.25f);
            Press(_keyboard.dKey);
            yield return new WaitForSeconds(0);
            Press(_keyboard.cKey);
            yield return new WaitForSeconds(1.0f);


            Assert.Greater(character.transform.position.x, blockadeLeft.transform.position.x, "Sliding right should have went under and beyond the right blockade");
        }

        [UnityTest]
        public IEnumerator CharacterThirdPerson_SlideForward_BeyondForwardBlockade()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            var originalPosition = Vector3.zero;
            var character = GameObject.FindObjectOfType<FirstPersonPlayer>();
            var blockadeForward = GameObject.Find("Blockade_Forward");
            character.transform.position = originalPosition;

            Press(_keyboard.qKey);
            yield return new WaitForSeconds(0.25f);
            Press(_keyboard.wKey);
            yield return new WaitForSeconds(0);
            Press(_keyboard.cKey);
            yield return new WaitForSeconds(1.0f);


            Assert.Greater(character.transform.position.z, blockadeForward.transform.position.z, "Sliding forward should have went under and beyond the forward blockade");
        }

        [UnityTest]
        public IEnumerator CharacterThirdPerson_SlideBackward_BeyondBackwardBlockade()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            var originalPosition = Vector3.zero;
            var character = GameObject.FindObjectOfType<FirstPersonPlayer>();
            var blockadeForward = GameObject.Find("Blockade_Backward");
            character.transform.position = originalPosition;

            Press(_keyboard.qKey);
            yield return new WaitForSeconds(0.25f);
            Press(_keyboard.sKey);
            yield return new WaitForSeconds(0);
            Press(_keyboard.cKey);
            yield return new WaitForSeconds(1.0f);


            Assert.Less(character.transform.position.z, blockadeForward.transform.position.z, "Sliding backward should have went under and beyond the backward blockade");
        }

        [UnityTest]
        public IEnumerator CharacterThirdPerson_SlideForwardRight_BeyondDiagonalForwardRightBlockade()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            var originalPosition = Vector3.zero;
            var character = GameObject.FindObjectOfType<FirstPersonPlayer>();
            var blockadeForward = GameObject.Find("Blockade_Forward");
            var blockadeRight = GameObject.Find("Blockade_Right");
            character.transform.position = originalPosition;

            Press(_keyboard.qKey);
            yield return new WaitForSeconds(0.25f);
            Press(_keyboard.wKey);
            Press(_keyboard.dKey);
            yield return new WaitForSeconds(0);
            Press(_keyboard.cKey);
            yield return new WaitForSeconds(1.0f);


            Assert.Greater(character.transform.position.z, blockadeForward.transform.position.z, "Sliding forward right should have went under and beyond the forward blockade");
            Assert.Greater(character.transform.position.x, blockadeRight.transform.position.x, "Sliding forward right should have went under and beyond the right blockade");
        }

        [UnityTest]
        public IEnumerator CharacterThirdPerson_SlideForwardLeft_BeyondDiagonalForwardLeftBlockade()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            var originalPosition = Vector3.zero;
            var character = GameObject.FindObjectOfType<FirstPersonPlayer>();
            var blockadeForward = GameObject.Find("Blockade_Forward");
            var blockadeLeft = GameObject.Find("Blockade_Left");
            character.transform.position = originalPosition;

            Press(_keyboard.qKey);
            yield return new WaitForSeconds(0.25f);
            Press(_keyboard.wKey);
            Press(_keyboard.aKey);
            yield return new WaitForSeconds(0);
            Press(_keyboard.cKey);
            yield return new WaitForSeconds(1.0f);


            Assert.Greater(character.transform.position.z, blockadeForward.transform.position.z, "Sliding forward left should have went under and beyond the forward blockade");
            Assert.Less(character.transform.position.x, blockadeLeft.transform.position.x, "Sliding forward left should have went under and beyond the left blockade");
        }

        [UnityTest]
        public IEnumerator CharacterThirdPerson_SlideBackwardLeft_BeyondDiagonalBackwardLeftBlockade()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            var originalPosition = Vector3.zero;
            var character = GameObject.FindObjectOfType<FirstPersonPlayer>();
            var blockadeBackward = GameObject.Find("Blockade_Backward");
            var blockadeLeft = GameObject.Find("Blockade_Left");
            character.transform.position = originalPosition;

            Press(_keyboard.qKey);
            yield return new WaitForSeconds(0.25f);
            Press(_keyboard.sKey);
            Press(_keyboard.aKey);
            yield return new WaitForSeconds(0);
            Press(_keyboard.cKey);
            yield return new WaitForSeconds(1.0f);


            Assert.Less(character.transform.position.z, blockadeBackward.transform.position.z, "Sliding backward left should have went under and beyond the forward blockade");
            Assert.Less(character.transform.position.x, blockadeLeft.transform.position.x, "Sliding backward left should have went under and beyond the left blockade");
        }

        [UnityTest]
        public IEnumerator CharacterThirdPerson_SlideBackwardRight_BeyondDiagonalBackwardRightBlockade()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            var originalPosition = Vector3.zero;
            var character = GameObject.FindObjectOfType<FirstPersonPlayer>();
            var blockadeBackward = GameObject.Find("Blockade_Backward");
            var blockadeRight = GameObject.Find("Blockade_Right");
            character.transform.position = originalPosition;

            Press(_keyboard.qKey);
            yield return new WaitForSeconds(0.25f);
            Press(_keyboard.sKey);
            Press(_keyboard.dKey);
            yield return new WaitForSeconds(0);
            Press(_keyboard.leftCtrlKey);
            yield return new WaitForSeconds(1.0f);

            Assert.Less(character.transform.position.z, blockadeBackward.transform.position.z, "Sliding backward right should have went under and beyond the forward blockade");
            Assert.Greater(character.transform.position.x, blockadeRight.transform.position.x, "Sliding backward right should have went under and beyond the right blockade");
        }
    }
}
