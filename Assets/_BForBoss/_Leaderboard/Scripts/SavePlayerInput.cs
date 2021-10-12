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

        private string Username;
        private int Timer;
        [SerializeField] private Text InputFieldUsername;
        [SerializeField] private Text InputFieldTimer;

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
        }

        public void StoreInput()
        {
            Username = InputFieldUsername.text;
            Timer = int.Parse(InputFieldTimer.text);
        }
    }
}
