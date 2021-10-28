using UnityEngine.InputSystem;

namespace BForBoss
{
    public class DetectInput
    {
        private int _controllerCount = 0;
        private int _mkbCount = 0;

        public void Reset()
        {
            _controllerCount = 0;
            _mkbCount = 0;
        }
        
        public void Detect()
        {
            if (Keyboard.current.anyKey.isPressed)
            {
                IncrementMkb();
            }
            else
            {
                IncrementController();
            }
        }

        private void IncrementController()
        {
            _controllerCount++;
        }

        private void IncrementMkb()
        {
            _mkbCount++;
        }

        public string GetInput()
        {
            if (_mkbCount > _controllerCount)
            {
                return "MKB";
            }

            return "Controller";
        }
    }
}