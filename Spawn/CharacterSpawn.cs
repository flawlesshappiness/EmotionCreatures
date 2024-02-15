using Godot;

public partial class CharacterSpawn : WorldSpawn
{
    [Export]
    public CharacterType CharacterType;

    [NodeName("Debug")]
    public Node3D DebugNode;

    public override void _Ready()
    {
        base._Ready();
        DebugNode.Hide();
    }

    public override void Spawn()
    {
        base.Spawn();

        var character = CharacterController.Instance.CreateCharacter(CharacterType);
        character.Transform = Transform;
        this.SetParent(character);
    }
}
