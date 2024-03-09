public partial class CreatureCombat : NodeScript
{
    [NodeType(typeof(HurtboxArea))]
    public HurtboxArea Hurtbox;

    private CreatureCharacter Creature { get; set; }

    public CreatureMove CurrentMove { get; set; }

    public void Initialize(CreatureCharacter creature)
    {
        Creature = creature;
        Hurtbox.OnHitCreature += OnHitCreature;
        DisableHitbox();
    }

    private void OnHitCreature(CreatureCharacter target, bool hit_before)
    {
        Debug.LogMethod();
        Debug.Indent++;

        if (CurrentMove == null)
        {
            Debug.LogError("CreatureCombat.CurrentMove is not set");
            Debug.Indent--;
            return;
        }

        if (!hit_before)
        {
            target.ApplyEffect(CurrentMove.Effect);
        }

        Debug.Indent--;
    }

    public void EnableHitbox()
    {
        Debug.LogMethod();
        Debug.Indent++;

        if (CurrentMove == null)
        {
            Debug.LogError("CreatureCombat.CurrentMove is not set");
            Debug.Indent--;
            return;
        }

        Hurtbox.SetPosition(CurrentMove.Info.MeleeHitboxPosition);
        Hurtbox.SetSize(CurrentMove.Info.MeleeHitboxSize);
        Hurtbox.SetEnabled(true);

        Debug.Indent--;
    }

    public void DisableHitbox()
    {
        Debug.LogMethod();
        Debug.Indent++;

        Hurtbox.SetEnabled(false);

        Debug.Indent--;
    }

    public void FireProjectile()
    {
        Debug.LogMethod();
        Debug.Indent++;

        if (CurrentMove == null)
        {
            Debug.LogError("CreatureCombat.CurrentMove is not set");
            Debug.Indent--;
            return;
        }

        var projectile = ProjectileController.Instance.CreateProjectile(CurrentMove.Info.ProjectileType);
        var direction = Creature.GlobalBasis.Z;

        projectile.SetParent(Scene.Current);
        projectile.GlobalPosition = Creature.GlobalPosition + direction * 1.0f;
        projectile.Sender = Creature;
        projectile.Effect = CurrentMove.Effect;

        projectile.Fire(direction, CurrentMove.Info.ProjectileSpeed);

        Debug.Indent--;
    }
}
