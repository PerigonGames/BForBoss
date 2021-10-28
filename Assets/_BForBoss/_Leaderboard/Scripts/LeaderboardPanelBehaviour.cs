using DG.Tweening;
using PerigonGames;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace BForBoss
{
    public class LeaderboardPanelBehaviour : MonoBehaviour
    {
        [SerializeField] private LeaderboardTableBehaviour _table = null;
        [SerializeField] private TMP_Text _loading = null;
        [SerializeField] private LeaderboardRowBehaviour _currentUserScores = null;
        
        private DreamloGetLeaderboardEndPoint _leaderboardEndpoint = new DreamloGetLeaderboardEndPoint();

        private void Awake()
        {
            transform.localScale = Vector3.zero;
            _leaderboardEndpoint.OnSuccess += HandleOnSuccess;
            _leaderboardEndpoint.OnFail += HandleOnFail;
        }

        public void SetUserTime(float time, string input)
        {
            var score = new LeaderboardScore();
            score.Input = input;
            score.Time = time;
            score.Username = PlayerPrefs.GetString(UploadPlayerScoreDataSource.PlayerPrefKey.UserName);
            _currentUserScores.SetField(-1, score);
        }

        [Button]
        public void ShowPanel()
        {
            Reload();
            transform.DOScale(Vector3.one, 0.5f);
        }

        private void HandleOnSuccess(LeaderboardScore[] scores)
        {
            StopAnimation();
            _table.transform.ResetScale();
            _table.SetScores(scores);
        }

        private void HandleOnFail()
        {
            _loading.DOPause();
            _loading.text = "Connection Failed";
        }

        private void Reload()
        {
            _table.gameObject.transform.localScale = Vector3.zero;
            _leaderboardEndpoint.GetLeaderboard();
            PlayAnimation();
        }

        private void PlayAnimation()
        {
            _loading.transform.ResetScale();
            _loading.transform.DOScale(Vector3.one * 0.5f, 1f).SetLoops(-1, LoopType.Yoyo).SetId("loading");
        }

        private void StopAnimation()
        {
            DOTween.Kill("loading");
            _loading.transform.localScale = Vector3.zero;
        }
    }
}
