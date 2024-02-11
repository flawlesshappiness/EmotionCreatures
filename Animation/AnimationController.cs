using Godot;

public partial class AnimationController : Node3DScript
{
    [Export]
    public string IdleAnimation;

    [Export]
    public string MoveAnimation;

    [NodeName("AnimationPlayer")]
    public AnimationPlayer Animation;

    protected Character Character { get; private set; }

    private AnimationEvent Idle, Move;

    public override void _Ready()
    {
        base._Ready();

        Idle = new AnimationEvent(IdleAnimation, Animation)
            .Loop();
        Move = new AnimationEvent(MoveAnimation, Animation)
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
}
