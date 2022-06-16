using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Tests.Character
{
    public class BaseCharacterTests : InputTestFixture
    {
        private const string CHARACTER_TEST_SCENE = "Assets/_BForBoss/Tests/Scenes/GenericCharacterTests.unity";
        protected Keyboard _keyboard = null;

        [SetUp]
        public virtual void Setup()
        {
            base.Setup();
            EditorSceneManager.LoadSceneAsyncInPlayMode(CHARACTER_TEST_SCENE, new LoadSceneParameters(LoadSceneMode.Single));
            _keyboard = InputSystem.AddDevice<Keyboard>();
        }
    }
}
