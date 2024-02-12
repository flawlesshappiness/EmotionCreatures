using Godot;
using System.Linq;

public partial class ArenaController : Node
{
    public static ArenaController Instance => Singleton.TryGet<ArenaController>(out var instance) ? instance : Create();

    public static ArenaController Create() =>
        Singleton.Create<ArenaController>($"Arena/{nameof(ArenaController)}");

    public ArenaScene CurrentArena { get; private set; }

    private ArenaInfoCollection _collection;
    private ArenaInfoCollection Collection => _collection ?? (_collection = LoadCollection());
    private ArenaInfoCollection LoadCollection() => ArenaInfoCollection.Load<ArenaInfoCollection>(ResourcePaths.Instance.Collection.ArenaInfoCollection);

    public ArenaScene SetArena(ArenaType type)
    {
        Debug.LogMethod(type);
        Debug.Indent++;
        RemoveArena();
        CurrentArena = CreateArena(type);
        Debug.Indent--;

        return CurrentArena;
    }

    private void RemoveArena()
    {
        if (CurrentArena != null)
        {
            Debug.LogMethod();
            Debug.Indent++;
            CurrentArena.QueueFree();
            CurrentArena = null;
            Debug.Indent--;
            Debug.Log($"Success");
        }
    }

    private ArenaScene CreateArena(ArenaType type)
    {
        Debug.LogMethod(type);
        Debug.Indent++;
        var info = GetRandomArenaInfo(type);
        var arena = Scene.Instantiate<ArenaScene>(info.Scene);
        arena.World.GlobalPosition = new Vector3(300, 0, 0);
        Debug.Indent--;
        Debug.Log($"Success: {arena}");
        return arena;
    }

    private ArenaInfo GetRandomArenaInfo(ArenaType type)
    {
        var arenas = Collection.Resources.Where(r => r.Type == type).ToList();
        return arenas.Random();
    }
}
