using UnityEngine;

namespace BForBoss
{
    public class LockMouseUtility
    {
        private static readonly LockMouseUtility _instance = new LockMouseUtility();
        public static LockMouseUtility Instance => _instance;

        static LockMouseUtility()
        {
        }

        private LockMouseUtility()
        {
        }
        
        public void LockMouse()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void UnlockMouse()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}