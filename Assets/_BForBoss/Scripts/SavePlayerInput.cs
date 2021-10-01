using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BForBoss
{
    public class SavePlayerInput : MonoBehaviour
    {
        public InputField UsernameField;
        public InputField TimerField;
        public Dropdown InputSelectField;

        public void clickUpload()
        {
            PlayerPrefs.SetString("name", UsernameField.text);
            Debug.Log("Your name is " + PlayerPrefs.GetString("name"));
        }
    }
}
