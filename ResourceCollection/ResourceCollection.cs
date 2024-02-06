using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class ResourceCollection<T> : Resource where T : Resource
{
    [Export(PropertyHint.File)]
    public string[] Paths { get; set; }

    private List<T> _resources;
    public List<T> Resources => _resources ?? (_resources = LoadAll());
    public List<T> LoadAll() => Paths.Select(path => GD.Load<T>(path)).ToList();
}
