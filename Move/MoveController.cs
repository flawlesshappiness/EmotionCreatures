using Godot;
using System.Linq;

public partial class MoveController : Node
{
    public static MoveController Instance => Singleton.TryGet<MoveController>(out var instance) ? instance : Create();
    public static MoveController Create() => Singleton.Create<MoveController>($"Move/{nameof(MoveController)}");

    private MoveInfoCollection _collection;
    private MoveInfoCollection Collection => _collection ?? (_collection = LoadCollection());
    private MoveInfoCollection LoadCollection() => MoveInfoCollection.Load<MoveInfoCollection>(ResourcePaths.Instance.Collection.MoveInfoCollection);

    public MoveInfo GetInfo(MoveType type)
    {
        Debug.TraceMethod(type);
        Debug.Indent++;

        var info = Collection.Resources.FirstOrDefault(x => x.Type == type);
        if (info == null)
        {
            Debug.LogError($"Found no MoveInfo with type: {type}");
        }

        Debug.Indent--;
        return info;
    }
}
