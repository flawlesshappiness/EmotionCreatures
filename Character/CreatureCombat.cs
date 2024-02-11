using Godot;

public partial class CreatureCombat : NodeScript
{
    [NodeName(nameof(MeleeAttackArea))]
    public Area3D MeleeAttackArea;

    private CharacterBody3D Body { get; set; }

    public void SetBody(CharacterBody3D body)
    {
        Body = body;
    }

    public void Attack()
    {
        Debug.LogMethod();
        Debug.Indent++;

        var bodies = MeleeAttackArea.GetOverlappingBodies();
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
