using Godot;

public partial class ResourcePaths : Node
{
    public static ResourcePaths Instance => Singleton.TryGet<ResourcePaths>(out var instance) ? instance : Create();

    public static ResourcePaths Create() =>
        Singleton.CreateSingleton<ResourcePaths>($"ResourcePaths/{nameof(ResourcePaths)}");

    private ResourcePathsCollection _collection;
    public ResourcePathsCollection Collection => _collection ?? (_collection = LoadCollection());

    private ResourcePathsCollection LoadCollection()
    {
        var path = $"res://ResourcePaths/Resources/{nameof(ResourcePathsCollection)}.tres";
        var collection = GD.Load<ResourcePathsCollection>(path);
        return collection;
    }
}
