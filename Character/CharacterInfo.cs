using Godot;

[GlobalClass]
public partial class CharacterInfo : Resource
{
    [Export(PropertyHint.File)]
    public string Scene { get; set; }

    [Export]
    public CharacterBaseType BaseType { get; set; }

    [Export]
    public CharacterType CharacterType { get; set; }
}
