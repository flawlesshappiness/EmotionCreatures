public partial class CreatureCharacter : Character
{
    public CreatureAnimationController CreatureAnimation { get; private set; }

    public override void _Ready()
    {
        base._Ready();

        CreatureAnimation = Animation as CreatureAnimationController;
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
        var anim = CreatureAnimation.Attack;

        Movement.MovementLock.AddLock(anim.Name);
        anim.Play(() =>
        {
            CreatureAnimation.PlayIdle();
            Movement.MovementLock.RemoveLock(anim.Name);
        });
    }

    public void Damage()
    {
        var anim = CreatureAnimation.Hurt;

        Movement.MovementLock.AddLock(anim.Name);
        anim.Play(() =>
        {
            CreatureAnimation.PlayIdle();
            Movement.MovementLock.RemoveLock(anim.Name);
        });
    }
}