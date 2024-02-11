using Godot;

[GlobalClass]
public partial class CharacterInfoCollection : ResourceCollection<CharacterInfo>
{
    [Export(PropertyHint.File)]
    public string HumanBase { get; set; }

    [Export(PropertyHint.File)]
    public string CreatureBase { get; set; }
}
