using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BForBoss
{
    public class TypeInput : MonoBehaviour
    {
        public string Username;
        public int Timer;
        public GameObject inputField1;
        public GameObject inputField2;

        public void StoreInput()
        {
            Username = inputField1.GetComponent<Text>().text;
            Timer = int.Parse(inputField2.GetComponent<Text>().text);
        }
    }
}
