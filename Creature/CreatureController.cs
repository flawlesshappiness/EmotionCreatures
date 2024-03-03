using System.Linq;

public partial class CreatureController : ResourceController<CreatureInfoCollection, CreatureInfo>
{
    public static CreatureController Instance => GetController<CreatureController>("Creature");
    public CreatureInfoCollection Collection => GetCollection(ResourcePaths.Instance.Collection.CreatureInfoCollection);

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

    public CreatureCharacter CreateCreature(CreatureData data)
    {
        Debug.TraceMethod();

        var info = GetInfo(data.CharacterType);
        if (info == null)
        {
            Debug.LogError($"Unable to create creature with type: {data.CharacterType}, no CreatureInfo found for type");
            return null;
        }

        var creature = CharacterController.Instance.CreateCharacter(data.CharacterType) as CreatureCharacter;
        if (creature == null)
        {
            Debug.LogError($"Unable to create creature with type: {data.CharacterType}, not a CreatureCharacter");
            return null;
        }

        creature.SetData(data);
        creature.SetInfo(info);

        return creature;
    }
}
