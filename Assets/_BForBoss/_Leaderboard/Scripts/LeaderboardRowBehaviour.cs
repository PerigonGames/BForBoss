using UnityEngine;
using TMPro;

namespace Perigon.Leaderboard
{
    public class LeaderboardRowBehaviour : MonoBehaviour
    {

        [SerializeField] private TMP_Text _rank = null;
        [SerializeField] private TMP_Text _username = null;
        [SerializeField] private TMP_Text _time = null;
        [SerializeField] private TMP_Text _input = null;
        [SerializeField] private TMP_Text _date = null;
        
        public void SetField(int rank, LeaderboardScore score)
        {
            _rank.text = rank.ToString();
            _username.text = score.Username;
            _time.text = score.Time.ToString();
            _input.text = score.Input;
            _date.text = score.Date.ToShortDateString();
        }

        private void OnEnable()
        {
            _rank.text = "--";
            _username.text = "--";
            _time.text = "--";
            _input.text = "--";
            _date.text = "--";
        }
    }
}
