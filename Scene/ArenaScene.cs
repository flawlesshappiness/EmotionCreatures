using Godot;

public partial class ArenaScene : Scene
{
    [NodeName(nameof(World))]
    public Node3D World;

    [NodeName(nameof(PlayerStart))]
    public Node3D PlayerStart;

    [NodeName(nameof(OpponentStart))]
    public Node3D OpponentStart;
}
