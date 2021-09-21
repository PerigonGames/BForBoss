using ECM2.Characters;
using UnityEngine;

namespace ECM2.Templates.SideScrollerTemplate
{
    public class MyCharacter : Character
    {
        // TODO Add your game custom code here...

        protected override void HandleInput()
        {
            // Should handle input ?

            if (actions == null)
                return;

            // Add horizontal input movement (in world space)

            Vector2 movementInput = GetMovementInput();

            Vector3 movementDirection = Vector3.right * movementInput.x;

            SetMovementDirection(movementDirection);
        }
    }
}
