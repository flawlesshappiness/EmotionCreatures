using Godot;

public partial class CreatureAnimationController : AnimationController
{
    [Export]
    public string AttackAnimation;

    [Export]
    public string HurtAnimation;

    public AnimationEvent Attack, Hurt;

    public override void SetModel(Node3D model)
    {
        base.SetModel(model);

        Attack = new AnimationEvent(AttackAnimation, this);
        Hurt = new AnimationEvent(HurtAnimation, this);
    }
}
