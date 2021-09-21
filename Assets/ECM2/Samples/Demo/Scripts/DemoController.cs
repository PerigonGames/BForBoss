using UnityEngine;
using UnityEngine.UI;

namespace ECM2.Demo
{
    public class DemoController : MonoBehaviour
    {
        public GameObject[] gameObjects;

        [Space(15f)]
        public Text titleText;
        public Text descriptionText;

        private readonly string[] _titles =
        {
            "Character",
            "Third Person Character",
            "First Person Character",
            "Agent Character"
        };

        private readonly string[] _descriptions =
        {
            "<b>WASD</b> to move\n" + "<b>Space</b> to Jump\n" + "<b>C</b> to crouch\n" + "<b>Left Shift</b> to sprint",
            "<b>WASD</b> to move\n" + "<b>Mouse</b> to look around\n" + "<b>Scrollbar wheel</b> to zoom in / out\n"  + "<b>Space</b> to Jump\n" + "<b>C</b> to crouch\n" + "<b>Left Shift</b> to sprint",
            "<b>WASD</b> to move\n" + "<b>Mouse</b> to look around\n" + "<b>Space</b> to Jump\n" + "<b>C</b> to crouch\n" + "<b>Left Shift</b> to sprint",
            "<b>WASD</b> to move\n" + "<b>Mouse click</b> to move\n" + "<b>Space</b> to Jump\n" + "<b>C</b> to crouch\n" + "<b>Left Shift</b> to sprint",
        };

        public void EnableCharacter()
        {
            foreach (GameObject go in gameObjects)
                go.SetActive(false);

            gameObjects[0].SetActive(true);

            titleText.text = _titles[0];
            descriptionText.text = _descriptions[0];
        }

        public void EnableThirdPersonCharacter()
        {
            foreach (GameObject go in gameObjects)
                go.SetActive(false);

            gameObjects[1].SetActive(true);

            titleText.text = _titles[1];
            descriptionText.text = _descriptions[1];
        }

        public void EnableFirstPersonCharacter()
        {
            foreach (GameObject go in gameObjects)
                go.SetActive(false);

            gameObjects[2].SetActive(true);

            titleText.text = _titles[2];
            descriptionText.text = _descriptions[2];
        }

        public void EnableAgentCharacter()
        {
            foreach (GameObject go in gameObjects)
                go.SetActive(false);

            gameObjects[3].SetActive(true);

            titleText.text = _titles[3];
            descriptionText.text = _descriptions[3];
        }

        private void Awake()
        {
            EnableCharacter();
        }
    }
}
