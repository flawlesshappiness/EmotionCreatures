using Godot;

[GlobalClass]
public partial class CharacterInfo : Resource
{
    [Export(PropertyHint.File)]
    public string Scene { get; set; }

    [Export]
    public CharacterType Type { get; set; }
}
