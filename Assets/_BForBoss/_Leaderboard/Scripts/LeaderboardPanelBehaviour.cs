using DG.Tweening;
using Perigon.Utility;
using PerigonGames;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Perigon.Leaderboard
{
    public class LeaderboardPanelBehaviour : MonoBehaviour
    {
        [SerializeField] private LeaderboardTableBehaviour _table = null;
        [SerializeField] private TMP_Text _loading = null;
        [SerializeField] private LeaderboardRowBehaviour _currentUserScores = null;
        [Title("Buttons")] 
        [SerializeField] private Button _reloadButton = null;
        
        private readonly DreamloGetLeaderboardEndPoint _leaderboardEndpoint = new DreamloGetLeaderboardEndPoint();

        private void Awake()
        {
            _leaderboardEndpoint.OnSuccess += HandleOnSuccess;
            _leaderboardEndpoint.OnFail += HandleOnFail;
            _reloadButton.onClick.AddListener(Reload);
        }

        private void OnDestroy()
        {
            _reloadButton.onClick.RemoveListener(Reload);        
        }

        public void SetUserScore(float time)
        {
            var score = new LeaderboardScore
            {
                Time = time / 1000,
                Username = PlayerPrefs.GetString(PlayerPrefKeys.LeaderboardSettings.USERNAME)
            };
            _currentUserScores.SetField(0, score);
        }

        private void OnEnable()
        {
            Reload();
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
