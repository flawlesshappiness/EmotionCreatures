public partial class CreatureCharacter : Character
{
    [NodeType(typeof(CreatureAnimator))]
    public CreatureAnimator CreatureAnimator;

    [NodeType(typeof(HealthBar))]
    public HealthBar HealthBar;

    [NodeType(typeof(CreatureCombat))]
    public CreatureCombat Combat;

    public Health Health { get; private set; }
    public CreatureData Data { get; private set; }
    public CreatureInfo Info { get; private set; }
    public CreatureStats Stats { get; private set; }
    public bool IsAlive => !Health.IsDead;
    public bool IsDead => Health.IsDead;

    public override void _Ready()
    {
        base._Ready();

        Combat.SetBody(this);
        CreatureAnimator = Animator as CreatureAnimator;
        HealthBar.Hide();
    }

    public void SetData(CreatureData data)
    {
        Data = data;
    }

    public void SetInfo(CreatureInfo info)
    {
        var level = 1; // TODO: Get from core
        Info = info;
        Stats = CreatureStats.FromLevel(info, level);
        Stats.Trace();

        Health = new Health(Stats.Health);
        Health.OnValueChanged += OnHealthChanged;
        HealthBar.SubscribeTo(Health);
    }

    public void PrepareForBattle()
    {
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
        Attack();
    }

    public void Attack()
    {
        if (IsDead) return;

        var anim = CreatureAnimator.Attack;

        if (Animator.CurrentAnimation.Name == anim.Name) return;

        Movement.MovementLock.AddLock(anim.Name);
        anim.Play(() =>
        {
            CreatureAnimator.PlayIdle();
            Movement.MovementLock.RemoveLock(anim.Name);
        });
    }

    public void Damage()
    {
        if (IsDead) return;

        Health.AdjustValue(-1);
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
        Movement.MovementLock.AddLock("Death");

        var anim = CreatureAnimator.Dead;

        Movement.MovementLock.AddLock(anim.Name);
        anim.Play(() =>
        {
        });
    }
}
