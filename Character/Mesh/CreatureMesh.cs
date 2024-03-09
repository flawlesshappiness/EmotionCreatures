using Godot;

public partial class CreatureMesh : CharacterMesh
{
    [Export]
    public string MeleeAttackAnimation;

    [Export]
    public string ProjectileAttackAnimation;

    [Export]
    public string HurtAnimation;

    [Export]
    public string DeathAnimation;

    #region ANIMATION EVENTS
    public void EventEnableHurtbox()
    {
        AnimationEvent(new AnimationEvent
        {
            Hurtbox = new()
            {
                Enabled = true,
            }
        });
    }

    public void EventDisableHurtbox()
    {
        AnimationEvent(new AnimationEvent
        {
            Hurtbox = new()
            {
                Enabled = false,
            }
        });
    }

    public void EventFireProjectile()
    {
        AnimationEvent(new AnimationEvent
        {
            Projectile = new()
            {
            }
        });
    }

    public void EventMoveForward(float speed)
    {
        AnimationEvent(new AnimationEvent
        {
            Movement = new()
            {
                AutoMoveEnabled = true,
                Direction = Vector3.Back,
                Speed = speed,
            }
        });
    }

    public void EventStopMoving()
    {
        AnimationEvent(new AnimationEvent
        {
            Movement = new()
            {
                AutoMoveEnabled = false,
            }
        });
    }
    #endregion
}
