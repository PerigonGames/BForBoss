using UnityEngine;
using UnityEngine.Events;

namespace Perigon.Utility
{
    /// <summary>
    /// Helper class for Animator events.
    /// Events can only call one public method on a class attached to the animator -
    /// this allows events to fire other actions tied to a Unity Event
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class AnimatorCallbackHelper : MonoBehaviour
    {
        public UnityEvent _animatorEventCallback;
        public UnityEvent<int> _animatorEventIntCallback;
        public UnityEvent<float> _animatorEventFloatCallback;
        public UnityEvent<string> _animatorEventStringCallback;
        public UnityEvent<object> _animatorEventObjectCallback;
        public UnityEvent<AnimationEvent> _animatorEventFullCallback;
        
        public void AnimatorEventCallback()
        {
            _animatorEventCallback?.Invoke();
        }
        
        public void AnimatorEventIntCallback(int value)
        {
            _animatorEventIntCallback?.Invoke(value);
        }

        public void AnimatorEventFloatCallback(float value)
        {
            _animatorEventFloatCallback?.Invoke(value);
        }
        
        public void AnimatorEventStringCallback(string value)
        {
            _animatorEventStringCallback?.Invoke(value);
        }
        
        public void AnimatorEventObjectCallback(object value)
        {
            _animatorEventObjectCallback?.Invoke(value);
        }
        
        public void AnimatorEventFullCallback(AnimationEvent value)
        {
            _animatorEventFullCallback?.Invoke(value);
        }
    }
}
