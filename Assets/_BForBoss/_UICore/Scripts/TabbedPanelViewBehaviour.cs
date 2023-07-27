using System;
using PerigonGames;
using UnityEngine;

namespace Perigon.UserInterface
{
    public class TabbedPanelViewBehaviour : MonoBehaviour
    {
        [SerializeField] private TabbedContent[] _tabbedContent = null;

        protected void Initialize()
        {
            if (_tabbedContent.IsNullOrEmpty())
            {
                Debug.LogWarning("Tabbed Content is null or empty");
            }

            BindAllTabs();
        }
        
        private void BindTab(TabbedContent tabbedContent)
        {
            tabbedContent.Tab.onClick.AddListener(() =>
            {
                TurnOnTab(tabbedContent.Content);
            });
        }

        private void OpenFirstTab()
        {
            if (!_tabbedContent.IsNullOrEmpty())
            {
                TurnOnTab(_tabbedContent[0].Content);
                _tabbedContent[0].Tab.Select();
            }
        }
        
        private void TurnOffAllContent()
        {
            foreach (var content in _tabbedContent)
            {
                content.Content.SetActive(false);
            }
        }

        private void BindAllTabs()
        {
            foreach (var tabbedContent in _tabbedContent)
            {
                BindTab(tabbedContent);
            }
        }

        private void TurnOnTab(GameObject content)
        {
            TurnOffAllContent();
            content.SetActive(true);
        }

        private void OnEnable()
        {
            TurnOffAllContent();
            OpenFirstTab();
        }
    }

    [Serializable]
    public struct TabbedContent
    {
        public TabButtonBehaviour Tab;
        public GameObject Content;
    }
}