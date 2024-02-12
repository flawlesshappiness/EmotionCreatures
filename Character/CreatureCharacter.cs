using System;

public partial class CreatureCharacter : Character
{
    public CreatureAnimator CreatureAnimator { get; private set; }
    public CreatureCombat Combat { get; private set; }

    public int Health = 3;

    private bool is_dead;

    public bool IsAlive => !is_dead;

    public bool IsDead => is_dead;

    public Action OnDeath;

    public override void _Ready()
    {
        base._Ready();

        Combat = this.GetNodeInChildren<CreatureCombat>();
        Combat.SetBody(this);

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

        Health--;

        if (Health <= 0)
        {
            Kill();
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

    public void Kill()
    {
        if (IsDead) return;

        is_dead = true;
        Movement.MovementLock.AddLock("Death");

        var anim = CreatureAnimator.Dead;

        Movement.MovementLock.AddLock(anim.Name);
        anim.Play(() =>
        {
        });

        OnDeath?.Invoke();
    }
}
