public partial class CreatureAnimator : CharacterAnimator
{
    public AnimationEvent Attack, Hurt, Dead;

    public override void SetMesh(CharacterMesh mesh)
    {
        base.SetMesh(mesh);

        var cm = Mesh as CreatureMesh;
        Attack = new AnimationEvent(cm.AttackAnimation, this);
        Hurt = new AnimationEvent(cm.HurtAnimation, this);
        Dead = new AnimationEvent(cm.DeathAnimation, this);
    }
}
