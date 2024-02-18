using Godot;

[GlobalClass]
public partial class ProjectileInfo : Resource
{
    [Export]
    public ProjectileType Type { get; set; }

    [Export(PropertyHint.File)]
    public string Scene { get; set; }
}
