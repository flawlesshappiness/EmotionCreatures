using Godot;

[GlobalClass]
public partial class ArenaInfo : Resource
{
    [Export]
    public ArenaType Type { get; set; }

    [Export(PropertyHint.File)]
    public string Scene { get; set; }
}
