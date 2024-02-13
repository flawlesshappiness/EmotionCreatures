using Godot;
using System.Linq;

public partial class CreatureController : Node
{
    public static CreatureController Instance => Singleton.TryGet<CreatureController>(out var instance) ? instance : Create();
    public static CreatureController Create() => Singleton.Create<CreatureController>($"Creature/{nameof(CreatureController)}");

    private CreatureInfoCollection _collection;
    private CreatureInfoCollection Collection => _collection ?? (_collection = LoadCollection());
    private CreatureInfoCollection LoadCollection() => CreatureInfoCollection.Load<CreatureInfoCollection>(ResourcePaths.Instance.Collection.CreatureInfoCollection);

    public CreatureInfo GetInfo(CharacterType type)
    {
        Debug.TraceMethod(type);
        var info = Collection.Resources.FirstOrDefault(x => x.CharacterType == type);

        if (info == null)
        {
            Debug.LogError($"Unable to find CreatureInfo with type: {type}");
        }

        return info;
    }

    public CreatureCharacter CreateCreature(CharacterType type)
    {
        Debug.TraceMethod(type);

        var info = GetInfo(type);
        if (info == null)
        {
            Debug.LogError($"Unable to create creature with type: {type}, no CreatureInfo found for type");
            return null;
        }

        var creature = CharacterController.Instance.CreateCharacter(type) as CreatureCharacter;
        if (creature == null)
        {
            Debug.LogError($"Unable to create creature with type: {type}, not a CreatureCharacter");
            return null;
        }

        creature.SetInfo(info);

        return creature;
    }
}
