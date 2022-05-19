using System.Collections;
using UnityEngine;

namespace Perigon.Utility
{
    public class LevelDesignFeedbackCaller : MonoBehaviour
    { 
#if UNITY_EDITOR
        private void OnGUI()
        {
            Event evt = Event.current;
            if (evt.isKey && evt.keyCode == LevelDesignFeedbackWindowListener.OpenEditorKeyCode)
            {
                StartCoroutine(OpenLevelDesignFeedbackWindow());
            }
        }

        private IEnumerator OpenLevelDesignFeedbackWindow()
        {
            yield return new WaitForEndOfFrame();
            Texture2D rawScreenShot = ScreenCapture.CaptureScreenshotAsTexture();
            int width = rawScreenShot.width;
            int height = rawScreenShot.height;
            
            Texture2D filteredScreenShot = new Texture2D(width, height, TextureFormat.ARGB32, false);
            filteredScreenShot.ReadPixels(new Rect(0,0,width, height), 0 ,0);
            filteredScreenShot.Apply();

            LevelDesignFeedbackWindowListener.OpenLevelDesignFeedbackWindow(filteredScreenShot);
        }
#endif
    }
}
