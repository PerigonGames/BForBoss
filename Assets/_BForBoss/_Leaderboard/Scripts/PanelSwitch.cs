using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BForBoss
{
    public class PanelSwitch : MonoBehaviour
    {
        [SerializeField] private GameObject CanvasToHide;
        [SerializeField] private GameObject CanvasToShow;
        private bool CanvasVisible;

        public void switchCanvas()
        {
            CanvasVisible = !CanvasVisible;
            CanvasToHide.SetActive(!CanvasVisible);
            CanvasToShow.SetActive(CanvasVisible);
        }
    }
}
