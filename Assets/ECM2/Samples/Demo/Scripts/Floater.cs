using ECM2.Characters;
using UnityEngine;

public class Floater : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        Character character = other.GetComponent<Character>();
        if (!character)
            return;

        if (character.IsOnGround())
            character.PauseGroundConstraint();

        character.AddForce(character.GetGravityVector() * -1.05f);
    }
}
