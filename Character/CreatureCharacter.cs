using Godot;

public partial class CreatureCharacter : Character
{
    [NodeType(typeof(CreatureAnimationStateMachine))]
    public CreatureAnimationStateMachine CreatureAnimator;

    [NodeType(typeof(HealthBar))]
    public HealthBar HealthBar;

    [NodeType(typeof(CreatureCombat))]
    public CreatureCombat Combat;

    [NodeType(typeof(CreatureMoves))]
    public CreatureMoves Moves;

    [NodeName(nameof(Collider))]
    public CollisionShape3D Collider;

    public TeamType Team { get; private set; }
    public Health Health { get; private set; }
    public CreatureData Data { get; private set; }
    public CreatureInfo Info { get; private set; }
    public CreatureStats Stats { get; private set; }
    public bool IsAlive => !Health.IsDead;
    public bool IsDead => Health.IsDead;
    public int Level => Data.Core?.Level ?? 1;

    public override void _Ready()
    {
        base._Ready();

        Moves.Initialize(this);
        Combat.Initialize(this);
        HealthBar.Hide();
    }

    public void SetData(CreatureData data)
    {
        Data = data;
        Moves.LoadMoves(data.Moveset);
    }

    public void SetInfo(CreatureInfo info)
    {
        Info = info;
        Stats = CreatureStats.FromLevel(info, Level);
        Stats.Trace();

        Health = new Health(Stats.Health);
        Health.OnValueChanged += OnHealthChanged;
        HealthBar.SubscribeTo(Health);

        Movement.Speed = Info.Speed;
    }

    public void PrepareForBattle(TeamType team)
    {
        Team = team;
        HealthBar.Show();
    }

    public override void BeginTarget()
    {
        base.BeginTarget();
        PlayerInput.Instance.Attack.OnPressed += AttackPressed;
    }

    public override void EndTarget()
    {
        base.EndTarget();
        PlayerInput.Instance.Attack.OnPressed -= AttackPressed;
    }

    private void AttackPressed()
    {
        Moves.TryUseSelectedMove();
    }

    public void ApplyEffect(MoveEffect effect)
    {
        if (IsDead) // They're already dead, silly
        {
            return;
        }

        if (effect.Sender == this) // Don't hit yourself, doofus
        {
            return;
        }

        if (Team == effect.Team) // Don't friendly fire, idiot
        {
            return;
        }

        if (effect.Damage > 0)
        {
            Health.AdjustValue(-effect.Damage);
        }
    }

    #region ANIMATION EVENT
    protected override void AnimationEvent_Movement(AnimationEvent.MovementArgs args)
    {
        base.AnimationEvent_Movement(args);
    }

    protected override void AnimationEvent_Hitbox(AnimationEvent.HurtboxArgs args)
    {
        base.AnimationEvent_Hitbox(args);

        if (args.Enabled)
        {
            Combat.EnableHitbox();
        }
        else
        {
            Combat.DisableHitbox();
        }
    }

    protected override void AnimationEvent_Projectile(AnimationEvent.ProjectileArgs args)
    {
        base.AnimationEvent_Projectile(args);

        Combat.FireProjectile();
    }
    #endregion
    #region HEALTH
    private void OnHealthChanged()
    {
        if (Health.IsDead)
        {
            OnDeath();
        }
        else
        {
            Hurt();
        }
    }

    private void Hurt()
    {
        if (IsDead) return;

        CreatureAnimator.TriggerHurt.Trigger();
    }

    private void OnDeath()
    {
        Movement.MovementLock.AddLock("Death");
        Movement.GravityLock.AddLock("Death");
        Collider.Disabled = true;

        CreatureAnimator.TriggerDeath.Trigger();
    }
    #endregion
}
