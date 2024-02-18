using Godot;

public abstract partial class ResourceController : Node
{

}

public abstract partial class ResourceController<T, C, R> : ResourceController
    where T : Node
    where C : ResourceCollection<R>
    where R : Resource
{
    protected static T GetController(string directory) => Singleton.TryGet<T>(out var instance) ? instance : Create(directory);
    private static T Create(string directory) => Singleton.Create<T>($"{directory}/{typeof(T).Name}");

    private C _collection;
    protected C GetCollection(string path) => _collection ?? (_collection = LoadCollection(path));
    private C LoadCollection(string path) => ResourceCollection<R>.Load<C>(path);
}
