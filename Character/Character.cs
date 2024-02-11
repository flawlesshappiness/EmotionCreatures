using Godot;
using System;

public partial class Character : CharacterBody3D
{
    public Action OnBeginTarget, OnEndTarget;

    public CharacterMovement Movement { get; private set; }
    public AnimationController Animation { get; private set; }
    public CharacterNavigation Navigation { get; private set; }
    public AI AI { get; private set; }

    public bool IsPlayer => PlayerController.Instance.TargetCharacter == this;

    public override void _Ready()
    {
        base._Ready();

        Movement = this.GetNodeInChildren<CharacterMovement>();
        Movement.Initialize(this);

        Animation = this.GetNodeInChildren<AnimationController>();
        Animation.Initialize(this);

        Navigation = this.GetNodeInChildren<CharacterNavigation>();
        Navigation.Initialize(this);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (AI != null)
        {
            AI.Process();
        }
    }

    public virtual void BeginTarget()
    {
        OnBeginTarget?.Invoke();
    }

    public virtual void EndTarget()
    {
        OnEndTarget?.Invoke();
    }

    public void SetAI(AI ai)
    {
        AI = ai;
        AI.Initialize(this);
    }
}
