using Godot;
using Godot.Collections;

public partial class EnvironmentRandomizer : Node3DScript
{
    [Export]
    public Vector2 RotationRange = new Vector2(0, 360f);

    [Export]
    public Vector2 ScaleRange = new Vector2(0.8f, 1.2f);

    [Export]
    public Array<PackedScene> Prefabs;

    [NodeType(typeof(MeshInstance3D))]
    public MeshInstance3D MeshInstance;

    private bool init;

    public override void _Ready()
    {
        base._Ready();

        MeshInstance.QueueFree();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (!init)
        {
            init = true;
            var prefab = Prefabs.PickRandom();
            var instance = prefab.Instantiate() as MeshInstance3D;
            instance.SetParent(this);
            instance.Position = Vector3.Zero;

            var rng = new RandomNumberGenerator();
            var scale = rng.RandfRange(ScaleRange.X, ScaleRange.Y);
            var rotation = rng.RandfRange(RotationRange.X, RotationRange.Y);

            instance.Rotation = new Vector3(0, rotation, 0);
            instance.Scale = Vector3.One * scale;
        }
    }
}
