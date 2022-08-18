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
        
        public void AnimatorEventCallback(int value)
        {
            _animatorEventIntCallback?.Invoke(value);
        }
        
        public void AnimatorEventCallback(float value)
        {
            _animatorEventFloatCallback?.Invoke(value);
        }
        
        public void AnimatorEventCallback(string value)
        {
            _animatorEventStringCallback?.Invoke(value);
        }
        
        public void AnimatorEventCallback(object value)
        {
            _animatorEventObjectCallback?.Invoke(value);
        }
        
        public void AnimatorEventCallback(AnimationEvent value)
        {
            _animatorEventFullCallback?.Invoke(value);
        }
    }
}
