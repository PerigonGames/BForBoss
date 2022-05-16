using System.Collections;
using NUnit.Framework;
using Perigon.Character;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests.Character
{
    public class CharacterWallRunTests : InputTestFixture
    {
        private const float ON_GROUND_Y_POSITION = 0.5f;

        private Keyboard _keyboard = null;
        private Vector3 _narrowCorridor = new Vector3(0, 0.5f, -10);
        private Vector3 _wideCorridor = new Vector3(5, 0.5f, -10);
        
        public override void Setup()
        {
            base.Setup();
            EditorSceneManager.LoadSceneAsyncInPlayMode("Assets/_BForBoss/Tests/Scenes/CharacterWallRunTest.unity", new LoadSceneParameters(LoadSceneMode.Single));
            _keyboard = InputSystem.AddDevice<Keyboard>();
        }
        
        [UnityTest]
        public IEnumerator Test_CharacterWallRun_RunOverTheHole()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForFixedUpdate();
            }
            
            var character = GameObject.FindObjectOfType<FirstPersonPlayer>();
            character.transform.position = _narrowCorridor;
            
            Press(_keyboard.wKey);
            Press(_keyboard.aKey);
            yield return new WaitForSeconds(0.1f);
            Press(_keyboard.spaceKey);
            yield return new WaitForFixedUpdate();
            Release(_keyboard.aKey);
            Release(_keyboard.spaceKey);
            yield return new WaitForSeconds(1.5f);
            Release(_keyboard.wKey);
            yield return new WaitForSeconds(1.0f);

            Assert.Greater(character.transform.position.z, 0, "Should wall run over 0, 0, 0");
        }
        
        [UnityTest]
        public IEnumerator Test_CharacterWallRun_Jump_WallRunOtherWall()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForFixedUpdate();
            }
            
            var character = GameObject.FindObjectOfType<FirstPersonPlayer>();
            character.transform.position = _narrowCorridor;
            
            //First Wall Run
            Press(_keyboard.wKey);
            Press(_keyboard.aKey);
            yield return new WaitForSeconds(0.1f);
            Press(_keyboard.spaceKey);
            yield return new WaitForFixedUpdate();
            Release(_keyboard.aKey);
            Release(_keyboard.spaceKey);
            yield return new WaitForSeconds(0.1f);
            
            //First Jump
            Press(_keyboard.spaceKey);
            yield return new WaitForFixedUpdate();
            Release(_keyboard.spaceKey);
            yield return new WaitForFixedUpdate();
            yield return new WaitForSeconds(2f);

            Assert.Greater(character.transform.position.z, 0, "Should wall run over 0, 0, 0");
        }
        
        [UnityTest]
        public IEnumerator Test_CharacterWallRun_WallToWallJump_JumpOverOnCeiling()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForFixedUpdate();
            }
            
            var character = GameObject.FindObjectOfType<FirstPersonPlayer>();
            character.transform.position = _wideCorridor;
            
            //First Wall Run
            Press(_keyboard.wKey);
            Press(_keyboard.aKey);
            yield return new WaitForSeconds(0.1f);
            Press(_keyboard.spaceKey);
            yield return new WaitForFixedUpdate();
            Release(_keyboard.aKey);
            Release(_keyboard.spaceKey);
            yield return new WaitForSeconds(0.5f);
            
            //First Jump
            Press(_keyboard.spaceKey);
            yield return new WaitForFixedUpdate();
            Release(_keyboard.spaceKey);
            yield return new WaitForFixedUpdate();
            yield return new WaitForSeconds(0.2f);
            
            //Second mid air jump
            Press(_keyboard.spaceKey);
            yield return new WaitForFixedUpdate();
            Release(_keyboard.spaceKey);
            yield return new WaitForFixedUpdate();
            yield return new WaitForSeconds(2f);

            Assert.Greater(character.transform.position.z, 0, "Should wall run over 0, 0, 0");
            Assert.Greater(character.transform.position.y, 4, "Double jump should allow player to be higher than 5 y position");
        }
        
        [UnityTest]
        public IEnumerator Test_CharacterWallRun_IntoWall_FallsOntoFloor()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForFixedUpdate();
            }
            
            var character = GameObject.FindObjectOfType<FirstPersonPlayer>();
            character.transform.position = _narrowCorridor;
            
            Press(_keyboard.wKey);
            Press(_keyboard.aKey);
            yield return new WaitForSeconds(0.1f);
            Press(_keyboard.spaceKey);
            yield return new WaitForFixedUpdate();
            Release(_keyboard.spaceKey);
            yield return new WaitForSeconds(3f);

            var yPositionWithinBounds = TestUtilities.WithinBounds(character.transform.position.y, ON_GROUND_Y_POSITION);
            Assert.Greater(character.transform.position.z, 0, "Should wall run over 0, 0, 0");
            Assert.IsTrue(yPositionWithinBounds, "Wall running hitting a wall should fall down");
        }
    }
}
