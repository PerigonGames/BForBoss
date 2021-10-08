using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BForBoss
{
    public class SavePlayerInput : MonoBehaviour
    {
        [SerializeField] private InputField UsernameField;
        [SerializeField] private Button UploadButton;

        private void Awake()
        {
            UsernameField.text = PlayerPrefs.GetString("name");
            UploadButton.onClick.AddListener(()=>
            {
                clickUpload();
            });
        }

        public void clickUpload()
        {
            PlayerPrefs.SetString("name", UsernameField.text);
            Debug.Log("Your name is " + PlayerPrefs.GetString("name"));
        }
    }
}
