using ECM2.Components;
using UnityEngine;
using UnityEngine.Playables;

namespace ECM2.Examples.Platforms.AnimatedPlatformExample
{
    /// <summary>
    /// Example of how animate a platform using a PlayableDirector, this is important in order to manually 'play' its animation
    /// and keep in sync with our execution order.
    /// 
    /// This extends PlatformMovement and override its Move method to update its position (and rotation if needed).
    /// </summary>

    public sealed class AnimatedPlatform : PlatformMovement
    {
        private PlayableDirector _director;

        protected override void OnMove()
        {
            // Update animation

            _director.time = Time.time % _director.duration;
            _director.Evaluate();

            // Update our position and rotation with the animated transform position and rotation

            position = transform.position;
            rotation = transform.rotation;
        }
        
        private void Awake()
        {
            _director = GetComponent<PlayableDirector>();
        }
    }
}
