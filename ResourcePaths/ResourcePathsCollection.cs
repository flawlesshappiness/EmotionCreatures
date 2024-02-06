using Godot;

[GlobalClass]
public partial class ResourcePathsCollection : Resource
{
    [Export(PropertyHint.File)]
    public string ArenaInfoCollection { get; set; }
}