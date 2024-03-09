using Godot;
using System;

public partial class CharacterMesh : Node3DScript
{
    [NodeType(typeof(AnimationPlayer))]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public string IdleAnimation;

    [Export]
    public string MoveAnimation;

    [NodeType(typeof(Skeleton3D))]
    public Skeleton3D Skeleton;

    public Action<AnimationEvent> OnAnimationEvent;

    #region ANIMATION EVENTS
    protected void AnimationEvent(AnimationEvent animation_event)
    {
        OnAnimationEvent?.Invoke(animation_event);
    }

    public void EventEnablePlayerMovement()
    {
        AnimationEvent(new AnimationEvent
        {
            Movement = new()
            {
                PlayerInputEnabled = true,
            }
        });
    }

    public void EventDisablePlayerMovement()
    {
        AnimationEvent(new AnimationEvent
        {
            Movement = new()
            {
                PlayerInputEnabled = false,
            }
        });
    }
    #endregion
}
