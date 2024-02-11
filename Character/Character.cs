using Godot;
using System;

public partial class Character : CharacterBody3D
{
    public Action OnBeginTarget, OnEndTarget;

    public CharacterMovement Movement;
    public AnimationController Animation;

    public bool IsPlayer => PlayerController.Instance.TargetCharacter == this;

    public override void _Ready()
    {
        base._Ready();

        Movement = this.GetNodeInChildren<CharacterMovement>();
        Movement.SetBody(this);

        Animation = this.GetNodeInChildren<AnimationController>();
        Animation.SetCharacter(this);
    }

    public virtual void BeginTarget()
    {
        OnBeginTarget?.Invoke();
    }

    public virtual void EndTarget()
    {
        OnEndTarget?.Invoke();
    }
}
