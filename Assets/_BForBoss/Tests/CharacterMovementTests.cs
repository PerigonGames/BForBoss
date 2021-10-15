using System.Collections;
using NUnit.Framework;
using UnityEditor.SceneManagement;
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
            EditorSceneManager.LoadSceneAsyncInPlayMode("Assets/_BFB/Scenes/TestingScenes/LazCheckPointManagerTests.unity", new LoadSceneParameters(LoadSceneMode.Single));
            _keyboard = InputSystem.AddDevice<Keyboard>();

        }

            [UnityTest]
        public IEnumerator Character_MoveForward()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}