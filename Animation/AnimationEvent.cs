using Godot;
using System;

public class AnimationEvent
{
    public string Name { get; private set; }
    public AnimationPlayer Player { get; private set; }
    public Animation Animation { get; private set; }

    private Action OnAnimationFinished;

    public AnimationEvent(string name, AnimationPlayer player)
    {
        Name = name;
        Player = player;

        Animation = Player.GetAnimation(name);
        Player.AnimationFinished += AnimationFinished;
    }

    private void AnimationFinished(StringName animName)
    {
        if (animName != Name) return;

        OnAnimationFinished?.Invoke();
        OnAnimationFinished = null;
    }

    public void Play(Action onFinished = null)
    {
        if (onFinished != null)
        {
            OnAnimationFinished += onFinished;
        }

        Player.Play(Name);
    }

    public AnimationEvent Loop()
    {
        Animation.LoopMode = Animation.LoopModeEnum.Linear;
        return this;
    }
}
