using System.Linq;

public partial class MoveController : ResourceController<MoveInfoCollection, MoveInfo>
{
    public static MoveController Instance => GetController<MoveController>("Move");
    public MoveInfoCollection Collection => GetCollection(ResourcePaths.Instance.Collection.MoveInfoCollection);

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
