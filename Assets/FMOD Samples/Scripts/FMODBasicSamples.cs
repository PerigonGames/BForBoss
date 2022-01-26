using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;

namespace Perigon.FMODExamples
{
    public class FMODBasicSamples : MonoBehaviour
    {
        private const string PARAM_NAME = "Pitch";
        
        [SerializeField] Slider _pitchSlider;
        
        private StudioEventEmitter _emitter;

        public void Start()
        {
            _emitter = GetComponent<StudioEventEmitter>();
        }

        public void PlayOneShot()
        {
            float randomPitch = Random.value;
            _emitter.Play(); // note - play clears cached parameter values. Call this, THEN set params!
            _emitter.SetParameter(PARAM_NAME, randomPitch);
            
            //If we don't care about parameters, we could add a Serialized EventReference to this script, and then call
            //RuntimeManager.PlayOneShot(); 
            // This is the FMOD equivalent of giving a script an audio clip and then playing it using an audio manager instead of a direct audio source,
            // but it really complicates parameters
        }
        
        public void PlayWithPitch()
        {
            var pitch = _pitchSlider.value;
            _emitter.Play(); // note - play clears cached parameter values. Call this, THEN set params!
            _emitter.SetParameter(PARAM_NAME, pitch);
        }

        public void OpenAdvancedSamples()
        {
            Application.OpenURL("https://www.fmod.com/resources/documentation-unity?version=2.02&page=examples.html");
        }
    }
}
