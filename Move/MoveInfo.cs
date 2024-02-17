using Godot;

[GlobalClass]
public partial class MoveInfo : Resource
{
    [Export]
    public MoveType Type { get; set; }

    [Export]
    public string Name { get; set; }

    [Export]
    public float Cooldown { get; set; }

    [Export(PropertyHint.Range, "0,1000,1")]
    public float Damage { get; set; }

    [Export]
    public MoveAnimationType AnimationType { get; set; }

    [Export]
    public Vector3 MeleeHitboxPosition { get; set; }

    [Export]
    public Vector3 MeleeHitboxSize { get; set; }
}
