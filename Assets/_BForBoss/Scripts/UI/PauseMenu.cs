using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace BForBoss
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private Button _resumeButton = null;
        [SerializeField] private Button _resetButton = null;
        [SerializeField] private Button _quitButton = null;
        [SerializeField] private Button _settingsButton = null;
        
        private IInputSettings _inputSettings;
        
        public void Initialize(IInputSettings inputSettings)
        {
            _inputSettings = inputSettings;
            
            _resumeButton.onClick.AddListener(ResumeGame);
            _resetButton.onClick.AddListener(ResetGame);
            _quitButton.onClick.AddListener(QuitGame);
            _settingsButton.onClick.AddListener(OpenSettings);
        }

        public void OpenPanel()
        {
            StateManager.Instance.SetState(State.Pause);
            LockCharacterFunctionality(_inputSettings);
        }

        public void ClosePanel()
        {
            ResumeGame();
        }

        private void ResumeGame()
        {
            UnlockCharacterFunctionality(_inputSettings);
            StateManager.Instance.SetState(State.Play);
        }
        
        private void ResetGame()
        {
            UnlockCharacterFunctionality(_inputSettings);
            StateManager.Instance.SetState(State.PreGame);
        }

        private void QuitGame()
        {
            Application.Quit();

#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#endif
        }

        private void OpenSettings()
        {
            //Open Settings Menu
        }
        
        private void LockCharacterFunctionality(IInputSettings inputSettings)
        {
            LockMouseUtility.Instance.UnlockMouse();
            inputSettings.DisableActions();
            gameObject.SetActive(true);
        }

        private void UnlockCharacterFunctionality(IInputSettings inputSettings)
        {
            LockMouseUtility.Instance.LockMouse();
            inputSettings.EnableActions();
            gameObject.SetActive(false);
        }
        
        private void OnDestroy()
        {
            _resumeButton.onClick.RemoveAllListeners();
            _resetButton.onClick.RemoveAllListeners();
            _quitButton.onClick.RemoveAllListeners();
            _settingsButton.onClick.RemoveAllListeners();
        }
    }
}