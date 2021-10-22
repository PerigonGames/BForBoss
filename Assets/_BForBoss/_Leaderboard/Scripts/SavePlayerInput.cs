using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BForBoss
{
    public class SavePlayerInput : MonoBehaviour
    {
        [SerializeField] private InputField UsernameField;
        [SerializeField] private TMP_InputField TimerField;
        [SerializeField] private Dropdown InputSelectField;
        [SerializeField] private Button UploadButton;

        private DreamloSendScoreEndPoint Endpoint = new DreamloSendScoreEndPoint(); //Creating new object of a class

        private void Awake()
        {
            UsernameField.text = PlayerPrefs.GetString("name");
            UploadButton.onClick.AddListener(() =>
            {
                clickUpload();
            });
        }

        public void clickUpload()
        {
            PlayerPrefs.SetString("name", UsernameField.text);
            Endpoint.SendScore(UsernameField.text, float.Parse(TimerField.text), InputSelectField.captionText.text); //Replace controller line with dropdown input
        }
    }
}
