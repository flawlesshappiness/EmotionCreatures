using Godot;

public partial class CharacterAnimator : Node3DScript
{
    protected Character Character { get; private set; }
    public CharacterMesh Mesh { get; private set; }
    public AnimationPlayer Animation { get; private set; }
    public AnimationEvent CurrentAnimation { get; private set; }

    private AnimationEvent Idle, Move;

    public virtual void Initialize(Character character)
    {
        Character = character;
        Character.Movement.OnMoveStart += OnMoveStart;
        Character.Movement.OnMoveEnd += OnMoveEnd;
        Character.OnBeginTarget += OnBeginTarget;
        Character.OnEndTarget += OnEndTarget;
    }

    public virtual void SetMesh(CharacterMesh mesh)
    {
        Mesh = mesh;
        Mesh.SetParent(this);
        Animation = mesh.GetNodeInChildren<AnimationPlayer>();

        Idle = new AnimationEvent(Mesh.IdleAnimation, this)
            .Loop();
        Move = new AnimationEvent(Mesh.MoveAnimation, this)
            .Loop();

        OnMovingChanged(false);
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
