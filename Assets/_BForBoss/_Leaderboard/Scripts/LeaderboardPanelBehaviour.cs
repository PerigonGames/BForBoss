using DG.Tweening;
using PerigonGames;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BForBoss
{
    public class LeaderboardPanelBehaviour : MonoBehaviour
    {
        [SerializeField] private LeaderboardTableBehaviour _table = null;
        [SerializeField] private TMP_Text _loading = null;
        [SerializeField] private LeaderboardRowBehaviour _currentUserScores = null;
        [Title("Buttons")] 
        [SerializeField] private Button _hideButton = null;
        [SerializeField] private Button _reloadButton = null;
        
        private DreamloGetLeaderboardEndPoint _leaderboardEndpoint = new DreamloGetLeaderboardEndPoint();
        private ILockMouseInput _input = null;
        
        public void Initialize(ILockMouseInput input)
        {
            _input = input;
        }
        
        private void Awake()
        {
            transform.localScale = Vector3.zero;
            _leaderboardEndpoint.OnSuccess += HandleOnSuccess;
            _leaderboardEndpoint.OnFail += HandleOnFail;
            _hideButton.onClick.AddListener(HidePanel);
            _reloadButton.onClick.AddListener(Reload);
        }

        private void OnDestroy()
        {
            _hideButton.onClick.RemoveListener(HidePanel);
            _reloadButton.onClick.RemoveListener(Reload);        }

        public void SetUserTime(float time, string input)
        {
            var score = new LeaderboardScore();
            score.Input = input;
            score.Time = time / 1000;
            score.Username = PlayerPrefs.GetString(UploadPlayerScoreDataSource.PlayerPrefKey.UserName);
            _currentUserScores.SetField(0, score);
        }

        public void ShowPanel()
        {
            _input.UnlockMouse();
            Reload();
            transform.DOScale(Vector3.one, 0.5f);
        }

        private void HidePanel()
        {
            _input.LockMouse();
            transform.DOScale(Vector3.zero, 0.5f);
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
