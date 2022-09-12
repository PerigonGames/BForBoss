using UnityEngine;

namespace BForBoss
{
    public class QuitGameUseCase
    {
        public void Execute()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }
    }
}
