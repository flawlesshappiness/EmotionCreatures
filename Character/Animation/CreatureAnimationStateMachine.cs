using FlawLizArt.Animation.StateMachine;

public partial class CreatureAnimationStateMachine : CharacterAnimationStateMachine
{
    public TriggerParameter TriggerAttackMelee, TriggerAttackProjectile, TriggerHurt, TriggerDeath;
    public StateNode AttackMelee, AttackProjectile, Hurt, Death;

    protected override void CreateParameters()
    {
        base.CreateParameters();

        TriggerAttackMelee = CreateParameter(nameof(TriggerAttackMelee));
        TriggerAttackProjectile = CreateParameter(nameof(TriggerAttackProjectile));
        TriggerHurt = CreateParameter(nameof(TriggerHurt));
        TriggerDeath = CreateParameter(nameof(TriggerDeath));
    }

    protected override void CreateNodes()
    {
        base.CreateNodes();

        var mesh = Mesh as CreatureMesh;
        AttackMelee = CreateNode(mesh.MeleeAttackAnimation);
        AttackProjectile = CreateNode(mesh.ProjectileAttackAnimation);
        Hurt = CreateNode(mesh.HurtAnimation);
        Death = CreateNode(mesh.DeathAnimation);
    }

    protected override void CreateConnections()
    {
        base.CreateConnections();

        Connect(AttackMelee, Idle);
        Connect(AttackProjectile, Idle);
        Connect(Hurt, Idle);

        ConnectFromAnyState(AttackMelee, TriggerAttackMelee.WhenTriggered());
        ConnectFromAnyState(AttackProjectile, TriggerAttackProjectile.WhenTriggered());

        ConnectFromAnyState(Hurt, TriggerHurt.WhenTriggered());
        ConnectFromAnyState(Death, TriggerDeath.WhenTriggered());
    }

    protected override void AddAnimations()
    {
        base.AddAnimations();

        AddAnimation(AttackMelee.Name, false);
        AddAnimation(AttackProjectile.Name, false);
        AddAnimation(Hurt.Name, false);
        AddAnimation(Death.Name, false);
    }
}
