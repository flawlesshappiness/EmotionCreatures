using Godot;
using System;

public partial class Character : CharacterBody3D
{
    public Action OnBeginTarget, OnEndTarget;

    [NodeType(typeof(CharacterMovement))]
    public CharacterMovement Movement;

    [NodeType(typeof(CharacterAnimator))]
    public CharacterAnimator Animator;

    [NodeType(typeof(CharacterNavigation))]
    public CharacterNavigation Navigation;
    public AI AI { get; private set; }

    public bool IsPlayer => PlayerController.Instance.TargetCharacter == this;

    public override void _Ready()
    {
        base._Ready();

        NodeScript.FindNodesFromAttribute(this, GetType());

        Movement.Initialize(this);
        Animator.Initialize(this);
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
