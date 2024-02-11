using Godot;

public partial class AnimationController : Node3DScript
{
    [Export]
    public string IdleAnimation;

    [Export]
    public string MoveAnimation;

    public AnimationPlayer Animation { get; private set; }

    protected Character Character { get; private set; }

    protected AnimationEvent CurrentAnimation { get; private set; }

    private AnimationEvent Idle, Move;

    public virtual void SetModel(Node3D model)
    {
        model.SetParent(this);
        Animation = model.GetNodeInChildren<AnimationPlayer>();

        Idle = new AnimationEvent(IdleAnimation, this)
            .Loop();
        Move = new AnimationEvent(MoveAnimation, this)
            .Loop();

        OnMovingChanged(false);
    }

    public void SetCharacter(Character character)
    {
        Character = character;
        Character.Movement.OnMoveStart += OnMoveStart;
        Character.Movement.OnMoveEnd += OnMoveEnd;
        Character.OnBeginTarget += OnBeginTarget;
        Character.OnEndTarget += OnEndTarget;
    }

    protected virtual void OnBeginTarget()
    {

    }

    protected virtual void OnEndTarget()
    {
        OnMoveEnd();
    }

    private void OnMoveStart() => OnMovingChanged(true);

    private void OnMoveEnd() => OnMovingChanged(false);

    protected void OnMovingChanged(bool moving)
    {
        if (moving)
        {
            Move.Play();
        }
        else
        {
            Idle.Play();
        }
    }

    public void PlayIdle()
    {
        OnMovingChanged(Character.Movement.IsMoving);
    }

    public void SetCurrentAnimation(AnimationEvent anim)
    {
        if (CurrentAnimation != null && !CurrentAnimation.Finished)
        {
            CurrentAnimation.Interrupt();
            CurrentAnimation = null;
        }

        CurrentAnimation = anim;
    }
}
