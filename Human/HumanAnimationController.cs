using Godot;

public partial class HumanAnimationController : Node3DScript
{
    [Export]
    public string IdleAnimation;

    [Export]
    public string MoveAnimation;

    [NodeName("AnimationPlayer")]
    public AnimationPlayer Animation;

    private TopDownController _controller;

    public override void _Ready()
    {
        base._Ready();

        _controller = this.GetNodeInParents<TopDownController>();
        _controller.OnMoveStart += OnMoveStart;
        _controller.OnMoveEnd += OnMoveEnd;

        OnMovingChanged(false);
    }

    private void OnMoveStart() => OnMovingChanged(true);

    private void OnMoveEnd() => OnMovingChanged(false);

    private void OnMovingChanged(bool moving)
    {
        var name = moving ? MoveAnimation : IdleAnimation;
        var anim = Animation.GetAnimation(name);
        anim.LoopMode = Godot.Animation.LoopModeEnum.Linear;
        Animation.Play(name);
    }
}
