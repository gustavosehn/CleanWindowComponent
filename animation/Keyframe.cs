using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CleanWindowComponent.animation;

public class KeyframeProperty
{
    public string PropertyName { get; set; }
    public Dictionary<int, object> Keyframes { get; set; } = new();

    public KeyframeProperty(string propertyName)
    {
        PropertyName = propertyName;
    }

    public void SetKeyframe(int frame, object value)
    {
        Keyframes[frame] = value;
    }

    public object? GetValueAtFrame(int frame)
    {
        if (!Keyframes.Any())
            return null;

        var sortedFrames = Keyframes.Keys.OrderBy(k => k).ToList();

        if (frame <= sortedFrames.First())
            return Keyframes[sortedFrames.First()];

        if (frame >= sortedFrames.Last())
            return Keyframes[sortedFrames.Last()];

        int prevFrame = sortedFrames.LastOrDefault(f => f <= frame);
        int nextFrame = sortedFrames.FirstOrDefault(f => f > frame);

        if (prevFrame == 0 && nextFrame == 0)
            return Keyframes[sortedFrames.First()];

        object prevValue = Keyframes[prevFrame];
        object nextValue = Keyframes[nextFrame];

        return Interpolate(prevValue, nextValue, prevFrame, nextFrame, frame);
    }

    private object Interpolate(object from, object to, int fromFrame, int toFrame, int currentFrame)
    {
        double progress = (double)(currentFrame - fromFrame) / (toFrame - fromFrame);

        if (from is double fromDouble && to is double toDouble)
            return fromDouble + (toDouble - fromDouble) * progress;

        if (from is int fromInt && to is int toInt)
            return (int)(fromInt + (toInt - fromInt) * progress);

        if (from is bool fromBool && to is bool toBool)
            return progress < 0.5 ? fromBool : toBool;

        if (from is System.Windows.Media.Color fromColor && to is System.Windows.Media.Color toColor)
        {
            byte lerp(byte a, byte b) => (byte)(a + (b - a) * progress);
            return System.Windows.Media.Color.FromArgb(
                lerp(fromColor.A, toColor.A),
                lerp(fromColor.R, toColor.R),
                lerp(fromColor.G, toColor.G),
                lerp(fromColor.B, toColor.B)
            );
        }

        return from;
    }
}

public class KeyframeAnimation
{
    public string Name { get; set; }
    public int TotalFrames { get; set; }
    public double FrameRate { get; set; } = 60;
    public bool IsLooping { get; set; } = false;
    public Dictionary<string, KeyframeProperty> Properties { get; set; } = new();

    private int _currentFrame = 0;
    private bool _isPlaying = false;
    private DateTime _startTime;
    private Action<Dictionary<string, object>>? _onFrameUpdate;
    private Action? _onComplete;

    public KeyframeAnimation(string name, int totalFrames)
    {
        Name = name;
        TotalFrames = totalFrames;
    }

    public void AddProperty(string propertyName)
    {
        if (!Properties.ContainsKey(propertyName))
            Properties[propertyName] = new KeyframeProperty(propertyName);
    }

    public void SetKeyframe(string propertyName, int frame, object value)
    {
        AddProperty(propertyName);
        Properties[propertyName].SetKeyframe(frame, value);
    }

    public void Play(Action<Dictionary<string, object>> onFrameUpdate = null, Action onComplete = null)
    {
        _isPlaying = true;
        _currentFrame = 0;
        _startTime = DateTime.Now;
        _onFrameUpdate = onFrameUpdate;
        _onComplete = onComplete;
    }

    public void Stop()
    {
        _isPlaying = false;
        _currentFrame = 0;
    }

    public void Pause()
    {
        _isPlaying = false;
    }

    public void Resume()
    {
        if (!_isPlaying)
        {
            _isPlaying = true;
            _startTime = DateTime.Now.AddMilliseconds(-_currentFrame * (1000 / FrameRate));
        }
    }

    public void Update()
    {
        if (!_isPlaying)
            return;

        double elapsedMs = (DateTime.Now - _startTime).TotalMilliseconds;
        int newFrame = (int)(elapsedMs / (1000 / FrameRate));

        if (newFrame > TotalFrames)
        {
            if (IsLooping)
            {
                _startTime = DateTime.Now;
                newFrame = 0;
                _currentFrame = 0;
            }
            else
            {
                _isPlaying = false;
                _currentFrame = TotalFrames;
                var state = GetCurrentState();
                _onFrameUpdate?.Invoke(state);
                _onComplete?.Invoke();
                return;
            }
        }

        _currentFrame = newFrame;
        var currentState = GetCurrentState();
        _onFrameUpdate?.Invoke(currentState);
    }

    public Dictionary<string, object> GetCurrentState()
    {
        var state = new Dictionary<string, object>();
        foreach (var prop in Properties)
        {
            state[prop.Key] = prop.Value.GetValueAtFrame(_currentFrame);
        }
        return state;
    }

    public bool IsPlaying => _isPlaying;
    public int CurrentFrame => _currentFrame;
}

public class Keyframe
{
    public static void ApplyPropertiesToObject(object target, Dictionary<string, object> properties)
    {
        foreach (var kvp in properties)
        {
            ApplyPropertyToObject(target, kvp.Key, kvp.Value);
        }
    }

    public static void ApplyPropertyToObject(object target, string propertyName, object value)
    {
        var property = target.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
        
        if (property != null && property.CanWrite)
        {
            try
            {
                property.SetValue(target, Convert.ChangeType(value, property.PropertyType));
            }
            catch { }
        }
    }

    public static KeyframeAnimation CreateAnimation(string name, int totalFrames)
    {
        return new KeyframeAnimation(name, totalFrames);
    }
}