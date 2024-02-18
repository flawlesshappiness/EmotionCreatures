using Godot;

public partial class CreatureCombat : NodeScript
{
    [NodeName(nameof(MeleeHitbox))]
    public Area3D MeleeHitbox;

    [NodeName(nameof(MeleeHitboxShape))]
    public CollisionShape3D MeleeHitboxShape;

    private CreatureCharacter Creature { get; set; }

    public CreatureMove CurrentMove { get; set; }

    public void Initialize(CreatureCharacter creature)
    {
        Creature = creature;

        if (Creature.Animator.Mesh != null)
        {
            SetMesh(Creature.Animator.Mesh as CreatureMesh);
        }
        else
        {
            Creature.CreatureAnimator.OnMeshSet += m => SetMesh(m as CreatureMesh);
        }
    }

    private void SetMesh(CreatureMesh mesh)
    {
        mesh.OnAnimationMeleeHit += MeleeHit;
        mesh.OnAnimationProjectileFire += ProjectileFire;
    }

    public void MeleeHit()
    {
        if (CurrentMove == null)
        {
            Debug.LogError("CreatureCombat.CurrentMove is not set");
            return;
        }

        Debug.LogMethod();
        Debug.Indent++;

        MeleeHitboxShape.Position = CurrentMove.Info.MeleeHitboxPosition;

        var shape = MeleeHitboxShape.Shape as BoxShape3D;
        shape.Size = CurrentMove.Info.MeleeHitboxPosition;

        var bodies = MeleeHitbox.GetOverlappingBodies();
        foreach (var body in bodies)
        {
            if (body == Creature) continue;
            Debug.Trace("Body: " + body);
            var creature = body.GetNodeInChildren<CreatureCharacter>();
            Debug.Trace("Creature: " + creature);
            if (creature == null) continue;

            creature.ApplyEffect(CurrentMove.Effect);
        }

        Debug.Indent--;
    }

    public void ProjectileFire()
    {
        if (CurrentMove == null)
        {
            Debug.LogError("CreatureCombat.CurrentMove is not set");
            return;
        }

        Debug.LogMethod();
        Debug.Indent++;

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
