public partial class CreatureCharacter : Character
{
    [NodeType(typeof(CreatureAnimator))]
    public CreatureAnimator CreatureAnimator;

    [NodeType(typeof(HealthBar))]
    public HealthBar HealthBar;

    [NodeType(typeof(CreatureCombat))]
    public CreatureCombat Combat;

    [NodeType(typeof(CreatureMoves))]
    public CreatureMoves Moves;

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

        Combat.Initialize(this);
        CreatureAnimator = Animator as CreatureAnimator;
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
        UseSelectedMove();
    }

    public void UseSelectedMove()
    {
        if (IsDead) return;

        if (Moves.TryUseSelectedMove())
        {
            var move = Moves.SelectedMove;
            var anim = CreatureAnimator.GetAttackAnimationEvent(move.Info.AnimationType);

            if (Animator.CurrentAnimation.Name == anim.Name) return;

            Combat.CurrentMove = move;

            Movement.MovementLock.AddLock(anim.Name);
            anim.Play(() =>
            {
                CreatureAnimator.PlayIdle();
                Movement.MovementLock.RemoveLock(anim.Name);
            });
        }
    }

    public void ApplyEffect(MoveEffect effect)
    {
        if (IsDead) return;
        if (effect.Sender == this) return; // Don't hit self
        if (Team == effect.Team) return; // Don't friendly fire

        if (effect.Damage > 0)
        {
            Health.AdjustValue(-effect.Damage);
        }
    }

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

        var anim = CreatureAnimator.Hurt;

        Movement.MovementLock.AddLock(anim.Name);
        anim.Play(() =>
        {
            CreatureAnimator.PlayIdle();
            Movement.MovementLock.RemoveLock(anim.Name);
        });
    }

    private void OnDeath()
    {
        Debug.Log("On death");

        Movement.MovementLock.AddLock("Death");

        var anim = CreatureAnimator.Dead;

        Movement.MovementLock.AddLock(anim.Name);
        anim.Play(() =>
        {
        });
    }
}
