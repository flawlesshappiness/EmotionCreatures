using Godot;

public partial class WorldObjectControl : ControlScript
{
    [NodeName(nameof(Origin))]
    public Node3D Origin;

    private Node3D current_object;

    public void Clear()
    {
        if (current_object == null) return;

        current_object.QueueFree();
        current_object = null;
    }

    public void SetObject(Node3D obj)
    {
        Clear();

        obj.SetParent(Origin);
        obj.GlobalPosition = Origin.GlobalPosition;
        obj.GlobalRotation = Origin.GlobalRotation;
    }

    public void LoadCreature(CreatureData data)
    {
        var creature = CreatureController.Instance.CreateCreature(data);
        SetObject(creature);
        creature.Movement.GravityLock.AddLock(nameof(TeamCreatureCard));
    }
}
