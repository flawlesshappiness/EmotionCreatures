public partial class CreatureCharacter : Character
{
    public CreatureAnimator CreatureAnimator { get; private set; }
    public CreatureCombat Combat { get; private set; }
    public Health Health { get; private set; }
    public bool IsAlive => !Health.IsDead;
    public bool IsDead => Health.IsDead;

    public override void _Ready()
    {
        base._Ready();

        Combat = this.GetNodeInChildren<CreatureCombat>();
        Combat.SetBody(this);

        Health = new Health(3);
        Health.OnValueChanged += OnHealthChanged;

        CreatureAnimator = Animator as CreatureAnimator;
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
