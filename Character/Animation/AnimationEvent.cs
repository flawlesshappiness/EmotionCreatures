using Godot;
using System;

public class AnimationEvent
{
    public string Name { get; private set; }
    public bool Finished { get; private set; }
    public CharacterAnimator Animator { get; private set; }
    public Animation Animation { get; private set; }

    private Action OnAnimationFinished;

    public AnimationEvent(string name, CharacterAnimator animator)
    {
        Name = name;
        Animator = animator;

        Animation = Animator.Animation.GetAnimation(name);
        Animator.Animation.AnimationFinished += AnimationFinished;
    }

    private void AnimationFinished(StringName animName)
    {
        if (animName != Name) return;
        Finish();
    }

    private void Finish()
    {
        Finished = true;
        OnAnimationFinished?.Invoke();
        OnAnimationFinished = null;
    }

    public void Interrupt()
    {
        Finish();
    }

    public void Play(Action onFinished = null)
    {
        Animator.SetCurrentAnimation(this);

        if (onFinished != null)
        {
            OnAnimationFinished += onFinished;
        }

        Finished = false;
        Animator.Animation.Play(Name);
    }

    public AnimationEvent Loop()
    {
        Animation.LoopMode = Animation.LoopModeEnum.Linear;
        return this;
    }
}
