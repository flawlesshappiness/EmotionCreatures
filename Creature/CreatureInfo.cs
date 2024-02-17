using Godot;

[GlobalClass]
public partial class CreatureInfo : Resource
{
    [Export]
    public CharacterType CharacterType { get; set; }

    [Export]
    public string Name { get; set; }

    [Export(PropertyHint.Range, "1,10000,1")]
    public float HealthMin { get; set; }

    [Export(PropertyHint.Range, "1,10000,1")]
    public float HealthMax { get; set; }
}
