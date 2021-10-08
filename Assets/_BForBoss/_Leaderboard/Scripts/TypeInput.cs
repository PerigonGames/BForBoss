using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BForBoss
{
    public class TypeInput : MonoBehaviour
    {
        private string Username;
        private int Timer;
        [SerializeField] private Text inputField1;
        [SerializeField] private Text inputField2;

        public void StoreInput()
        {
            Username = inputField1.text;
            Timer = int.Parse(inputField2.text);
        }
    }
}
