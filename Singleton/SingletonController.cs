using Godot;

public partial class SingletonController<T> : Node
    where T : Node
{
    protected static T GetController(string directory) => Singleton.TryGet<T>(out var instance) ? instance : Create(directory);
    private static T Create(string directory) => Singleton.Create<T>($"{directory}/{typeof(T).Name}");
}
