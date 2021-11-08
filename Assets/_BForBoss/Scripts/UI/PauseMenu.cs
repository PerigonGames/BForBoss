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


        private void OnEnable()
        {
            _resumeButton.onClick.AddListener(ResumeGame);
            _resetButton.onClick.AddListener(ResetGame);
            _quitButton.onClick.AddListener(QuitGame);
            _settingsButton.onClick.AddListener(OpenSettings);
        }

        private void ResumeGame()
        {
            StateManager.Instance.SetState(State.Play);
            gameObject.SetActive(false);
        }

        private void QuitGame()
        {
            Application.Quit();

#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#endif
        }

        private void ResetGame()
        {
            StateManager.Instance.SetState(State.PreGame);
        }

        private void OpenSettings()
        {
            //Open Settings Menu
        }


        private void OnDisable()
        {
            _resumeButton.onClick.RemoveAllListeners();
            _resetButton.onClick.RemoveAllListeners();
            _quitButton.onClick.RemoveAllListeners();
            _settingsButton.onClick.RemoveAllListeners();
        }
    }
}
