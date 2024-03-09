using FlawLizArt.Animation.StateMachine;
using Godot;
using System.Collections.Generic;

public partial class CharacterAnimationStateMachine : AnimationStateMachine
{
    public Character Character { get; private set; }
    public CharacterMesh Mesh { get; private set; }
    public AnimationPlayer Animator { get; private set; }

    public FloatParameter HorizontalVelocity;
    public StateNode Idle, Move;

    public Dictionary<string, AnimationState> Animations = new();

    public void Initialize(Character character)
    {
        Character = character;
        Mesh = character.Mesh;
        Animator = Mesh.AnimationPlayer;
        Animator.AnimationFinished += AnimationFinished;

        CreateParameters();
        CreateNodes();
        CreateConnections();
        AddAnimations();

        Start(Idle);
    }

    protected virtual void CreateParameters()
    {
        HorizontalVelocity = CreateParameter(nameof(HorizontalVelocity), 0f);
    }

    protected virtual void CreateNodes()
    {
        Idle = CreateNode(Mesh.IdleAnimation);
        Move = CreateNode(Mesh.MoveAnimation);
    }

    protected virtual void CreateConnections()
    {
        Connect(Idle, Move, HorizontalVelocity.When(ComparisonType.GreaterThan, 0.5f));
        Connect(Move, Idle, HorizontalVelocity.When(ComparisonType.LessThan, 0.5f));
    }

    protected virtual void AnimationFinished(StringName animName)
    {
        TryProcessCurrentState(true);
    }

    protected virtual void AddAnimations()
    {
        AddAnimation(Idle.Name, true);
        AddAnimation(Move.Name, true);
    }

    protected void AddAnimation(string animation, bool looping)
    {
        Animations.Add(animation, new AnimationState(animation)
        {
            Looping = looping
        });
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        var cvel = Character.Velocity;
        var vel = new Vector3(cvel.X, 0, cvel.Z);
        HorizontalVelocity.Set(vel.Length());
    }

    public override void SetCurrentNode(StateNode node)
    {
        base.SetCurrentNode(node);

        if (Animations.TryGetValue(node.Name, out var state))
        {
            var animation = Animator.GetAnimation(node.Name);
            animation.LoopMode = state.Looping ? Animation.LoopModeEnum.Linear : Animation.LoopModeEnum.None;
        }

        Animator.Play(node.Name);
    }
}
