using System;

public partial class CreatureAnimator : CharacterAnimator
{
    public AnimationEvent MeleeAttack, ProjectileAttack, Hurt, Dead;

    public Action<CharacterMesh> OnMeshSet;

    public override void SetMesh(CharacterMesh mesh)
    {
        base.SetMesh(mesh);

        var cm = Mesh as CreatureMesh;
        MeleeAttack = new AnimationEvent(cm.MeleeAttackAnimation, this);
        ProjectileAttack = new AnimationEvent(cm.ProjectileAttackAnimation, this);
        Hurt = new AnimationEvent(cm.HurtAnimation, this);
        Dead = new AnimationEvent(cm.DeathAnimation, this);

        OnMeshSet?.Invoke(mesh);
    }

    public AnimationEvent GetAttackAnimationEvent(MoveAnimationType type) => type switch
    {
        MoveAnimationType.Melee => MeleeAttack,
        MoveAnimationType.Projectile => ProjectileAttack,
        _ => null
    };
}
