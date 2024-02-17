using Godot;

public partial class CreatureCombat : NodeScript
{
    [NodeName(nameof(MeleeHitbox))]
    public Area3D MeleeHitbox;

    [NodeName(nameof(MeleeHitboxShape))]
    public CollisionShape3D MeleeHitboxShape;

    private CharacterBody3D Body { get; set; }

    public CreatureMove CurrentMove { get; set; }

    public void SetBody(CharacterBody3D body)
    {
        Body = body;
    }

    public void Attack()
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
            if (body == Body) continue;
            Debug.Trace("Body: " + body);
            var creature = body.GetNodeInChildren<CreatureCharacter>();
            Debug.Trace("Creature: " + creature);
            if (creature == null) continue;

            creature.Damage();
        }

        Debug.Indent--;
    }
}
