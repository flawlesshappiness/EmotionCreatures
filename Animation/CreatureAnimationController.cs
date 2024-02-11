using Godot;

public partial class CreatureAnimationController : AnimationController
{
    [Export]
    public string AttackAnimation;

    [Export]
    public string HurtAnimation;

    public AnimationEvent Attack, Hurt;

    public override void _Ready()
    {
        base._Ready();

        Attack = new AnimationEvent(AttackAnimation, Animation);
        Hurt = new AnimationEvent(HurtAnimation, Animation);
    }
}
