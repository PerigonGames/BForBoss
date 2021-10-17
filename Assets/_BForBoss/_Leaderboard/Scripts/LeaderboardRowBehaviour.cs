using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BForBoss
{
    public class LeaderboardRowBehaviour : MonoBehaviour
    {

        [SerializeField] private TMP_Text Rank; //TMP_Text
        [SerializeField] private TMP_Text Username;
        [SerializeField] private TMP_Text Time;
        [SerializeField] private TMP_Text Input;

        public void SetField(LeaderboardScore score)
        {
            Rank.text = "//";
            Username.text = score.Username;
            Time.text = score.Time.ToString();
            Input.text = score.Input;
        }
    }
}
