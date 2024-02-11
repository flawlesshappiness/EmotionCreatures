using Godot;

public partial class CreatureCombat : Node
{
    [NodeName(nameof(MeleeAttackArea))]
    public Area3D MeleeAttackArea;

    public void Attack()
    {
        Debug.LogMethod();
        Debug.Indent++;

        var bodies = MeleeAttackArea.GetOverlappingBodies();
        foreach (var body in bodies)
        {
            Debug.Log("Body: " + body);
            var creature = body.GetNodeInChildren<CreatureCharacter>();
            Debug.Log("Creature: " + creature);
            if (creature == null) continue;

            creature.Damage();
        }

        Debug.Indent--;
    }
}
