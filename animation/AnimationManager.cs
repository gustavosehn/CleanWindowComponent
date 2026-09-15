using System;
using System.Collections.Generic;
using System.Linq;

namespace CleanWindowComponent.animation;

public class AnimationManager
{
    private Dictionary<string, KeyframeAnimation> _animations = new();
    public List<KeyframeAnimation> PlayingAnimations { get; private set; } = new();

    public void RegisterAnimation(KeyframeAnimation animation)
    {
        _animations[animation.Name] = animation;
    }

    public void PlayAnimation(string animationName, object target, Action? onComplete = null)
    {
        if (_animations.TryGetValue(animationName, out var animation))
        {
            var newAnimation = CloneAnimation(animation);
            newAnimation.Play(state =>
            {
                Keyframe.ApplyPropertiesToObject(target, state);
            }, () =>
            {
                PlayingAnimations.Remove(newAnimation);
                onComplete?.Invoke();
            });
            PlayingAnimations.Add(newAnimation);
        }
    }

    public void StopAnimation(string animationName)
    {
        var toRemove = PlayingAnimations.Where(a => a.Name == animationName).ToList();
        foreach (var anim in toRemove)
        {
            anim.Stop();
            PlayingAnimations.Remove(anim);
        }
    }

    public void Update()
    {
        foreach (var animation in PlayingAnimations.ToList())
        {
            animation.Update();
        }
    }

    private KeyframeAnimation CloneAnimation(KeyframeAnimation original)
    {
        var clone = new KeyframeAnimation(original.Name, original.TotalFrames)
        {
            FrameRate = original.FrameRate,
            IsLooping = original.IsLooping
        };

        foreach (var prop in original.Properties)
        {
            clone.AddProperty(prop.Key);
            foreach (var kf in prop.Value.Keyframes)
            {
                clone.SetKeyframe(prop.Key, kf.Key, kf.Value);
            }
        }

        return clone;
    }
}