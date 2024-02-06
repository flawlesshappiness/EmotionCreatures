using Godot;
using System.Linq;

public partial class ArenaController : Node
{
    public static ArenaController Instance => Singleton.TryGet<ArenaController>(out var instance) ? instance : Create();

    public static ArenaController Create() =>
        Singleton.CreateSingleton<ArenaController>($"Arena/{nameof(ArenaController)}");

    public ArenaScene CurrentArena { get; private set; }

    private ArenaInfoCollection _collection;
    private ArenaInfoCollection Collection => _collection ?? (_collection = LoadCollection());

    private ArenaInfoCollection LoadCollection() => GD.Load<ArenaInfoCollection>(ResourcePaths.Instance.Collection.ArenaInfoCollection);
    private ArenaInfo GetInfo(ArenaType type) => Collection.Resources.FirstOrDefault(r => r.Type == type);

    public void SetArena(ArenaType type)
    {
        Debug.Indent++;
        Debug.Log($"Set Arena: {type}");
        RemoveArena();
        CurrentArena = CreateArena(type);
        Debug.Indent--;
    }

    private void RemoveArena()
    {
        Debug.Indent++;
        if (CurrentArena != null)
        {
            Debug.Log($"Removing current arena");
            CurrentArena.QueueFree();
            CurrentArena = null;
            Debug.Log($"Success");
        }
        Debug.Indent--;
    }

    private ArenaScene CreateArena(ArenaType type)
    {
        Debug.Indent++;
        Debug.Log($"Creating arena: {type}");
        Debug.Indent++;
        var info = GetInfo(type);
        var arena = Scene.CreateInstance<ArenaScene>(info.Scene);
        arena.World.GlobalPosition = new Vector3(300, 0, 0);
        Debug.Indent--;
        Debug.Log($"Success: {arena}");
        Debug.Indent--;
        return arena;
    }
}
