using Godot;
using System;

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

    public Action OnAnimationMeleeHit;
    public Action OnAnimationProjectileFire;

    public void AnimationMeleeHit()
    {
        OnAnimationMeleeHit?.Invoke();
    }

    public void AnimationProjectileFire()
    {
        OnAnimationProjectileFire?.Invoke();
    }
}
