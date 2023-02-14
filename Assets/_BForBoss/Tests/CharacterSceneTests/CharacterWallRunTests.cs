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
    public class CharacterWallRunTests : InputTestFixture
    {
        private const float ON_GROUND_Y_POSITION = 0.5f;

        private Keyboard _keyboard = null;
        private Vector3 _narrowCorridor => GameObject.Find("NarrowWallRunSpawn").transform.position;
        private Vector3 _wideCorridor =>GameObject.Find("WideWallRunSpawn").transform.position;
        
        public override void Setup()
        {
            base.Setup();
            EditorSceneManager.LoadSceneAsyncInPlayMode("Assets/_BForBoss/Tests/Scenes/GenericCharacterTests.unity", new LoadSceneParameters(LoadSceneMode.Single));
            _keyboard = InputSystem.AddDevice<Keyboard>();
        }

        [UnityTest]
        public IEnumerator Test_CharacterWallRun_Jump_WallRunOtherWall()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForFixedUpdate();
            }
            
            var character = GameObject.FindObjectOfType<PlayerMovementBehaviour>();
            character.transform.position = _narrowCorridor;
            
            //First Wall Run
            Press(_keyboard.wKey);
            Press(_keyboard.aKey);
            yield return new WaitForFixedUpdate();
            Press(_keyboard.spaceKey);
            yield return new WaitForFixedUpdate();
            yield return new WaitForSecondsRealtime(0.2f);
            Release(_keyboard.spaceKey);
            
            Release(_keyboard.aKey);
            yield return new WaitForFixedUpdate();
            yield return new WaitForSecondsRealtime(0.5f);
            
            //First Jump
            Press(_keyboard.spaceKey);
            yield return new WaitForFixedUpdate();
            yield return new WaitForSecondsRealtime(0.2f);
            Release(_keyboard.spaceKey);
            yield return new WaitForFixedUpdate();
            yield return new WaitForSecondsRealtime(3f);

            Assert.Greater(character.transform.position.y, 0, "Should wall run over 0, 0, 0");
        }
        
        [UnityTest]
        public IEnumerator Test_CharacterWallRun_WallToWallJump_JumpOverOnCeiling()
        {
            for (int i = 0; i < 50; i++)
            {
                yield return new WaitForFixedUpdate();
            }
            
            var character = GameObject.FindObjectOfType<PlayerMovementBehaviour>();
            character.transform.position = _narrowCorridor;
            //First Wall Run
            Press(_keyboard.wKey);
            Press(_keyboard.aKey);
            yield return new WaitForSecondsRealtime(0.1f);
            Press(_keyboard.spaceKey);
            yield return new WaitForSecondsRealtime(0.3f);
            Release(_keyboard.spaceKey);
            yield return new WaitForSecondsRealtime(0.3f);
            Release(_keyboard.aKey);
            yield return new WaitForSecondsRealtime(0.3f);
            
            //First Jump
            Press(_keyboard.spaceKey);
            yield return new WaitForSecondsRealtime(0.3f);
            Release(_keyboard.spaceKey);
            yield return new WaitForSecondsRealtime(0.3f);
            
            //Second mid air jump
            Press(_keyboard.spaceKey);
            yield return new WaitForSecondsRealtime(0.3f);
            Release(_keyboard.spaceKey);
            yield return new WaitForSecondsRealtime(2f);

            Assert.Greater(character.transform.position.z, 0, "Should wall run over 0, 0, 0");
            Assert.Greater(character.transform.position.y, 4, "Double jump should allow player to be higher than 5 y position");
        }
        
        /*
        [UnityTest]
        public IEnumerator Test_CharacterWallRun_IntoWall_FallsOntoFloor()
        {
            for (int i = 0; i < 50; i++)
            {
                yield return new WaitForFixedUpdate();
            }
            
            var character = GameObject.FindObjectOfType<PlayerMovementBehaviour>();
            character.transform.position = _wideCorridor;
            
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
        */
    }
}
