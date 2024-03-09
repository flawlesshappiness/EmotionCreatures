using Godot;
using System;
using System.Collections.Generic;

public partial class HurtboxArea : Area3D
{
    public const bool DEBUG = true;

    [NodeName(nameof(DebugBox))]
    public MeshInstance3D DebugBox;

    [NodeType(typeof(CollisionShape3D))]
    public CollisionShape3D CollisionShape;

    public Action<CreatureCharacter, bool> OnHitCreature;

    private BoxShape3D shape;
    private bool enabled;

    private List<Node3D> previous_hits = new();

    public override void _Ready()
    {
        base._Ready();
        NodeScript.FindNodesFromAttribute(this, GetType());
        shape = CollisionShape.Shape as BoxShape3D;

        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node3D body)
    {
        TryHit(body);
    }

    public void SetSize(Vector3 size)
    {
        shape.Size = size;
        DebugBox.Scale = size;
    }

    public void SetPosition(Vector3 position)
    {
        CollisionShape.Position = position;
    }

    public void SetEnabled(bool enabled)
    {
        this.enabled = enabled;
        DebugBox.Visible = enabled && DEBUG;

        previous_hits.Clear();

        if (enabled)
        {
            FindHits();
        }
    }

    private void FindHits()
    {
        var bodies = GetOverlappingBodies();
        foreach (var body in bodies)
        {
            TryHit(body);
        }
    }

    private void TryHit(Node3D body)
    {
        if (!enabled) return;

        var hit_before = previous_hits.Contains(body);

        var creature = body.GetNodeInChildren<CreatureCharacter>();
        if (creature != null)
        {
            OnHitCreature?.Invoke(creature, hit_before);
        }

        previous_hits.Add(body);
    }
}