using System;
using PerigonGames;
using UnityEngine;

namespace BForBoss
{
    public sealed class TabbedPanelViewBehaviour : MonoBehaviour
    {
        [SerializeField] private TabbedContent[] _tabbedContent = null;

        private void Awake()
        {
            if (_tabbedContent.IsNullOrEmpty())
            {
                Debug.LogWarning("Tabbed Content is null or empty");
            }

            BindAllTabs();
            OpenFirstTab();
        }

        private void OpenFirstTab()
        {
            if (!_tabbedContent.IsNullOrEmpty())
            {
                TurnOnTab(_tabbedContent[0].Content);
            }
        }

        private void BindAllTabs()
        {
            foreach (var tabbedContent in _tabbedContent)
            {
                BindTab(tabbedContent);
            }
        }

        private void BindTab(TabbedContent tabbedContent)
        {
            tabbedContent.Tab.onClick.AddListener(() =>
            {
                TurnOnTab(tabbedContent.Content);
            });
        }

        private void TurnOnTab(GameObject content)
        {
            TurnOffAllContent();
            content.SetActive(true);
        }

        private void TurnOffAllContent()
        {
            foreach (var content in _tabbedContent)
            {
                content.Content.SetActive(false);
            }
        }
    }

    [Serializable]
    public struct TabbedContent
    {
        public TabButtonBehaviour Tab;
        public GameObject Content;
    }
}