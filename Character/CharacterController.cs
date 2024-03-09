using Godot;
using System.Linq;

public partial class CharacterController : ResourceController<CharacterInfoCollection, CharacterInfo>
{
    public static CharacterController Instance => GetController<CharacterController>("Character");
    public CharacterInfoCollection Collection => GetCollection(ResourcePaths.Instance.Collection.CharacterInfoCollection);

    public Character CreateCharacter(CharacterType type)
    {
        Debug.LogMethod(type);
        Debug.Indent++;
        var info = Collection.Resources.FirstOrDefault(x => x.CharacterType == type);

        if (info == null)
        {
            Debug.LogError($"Found no character with type: {type}");
            return null;
        }

        var base_character = LoadBaseCharacter(info.BaseType);
        Debug.Trace($"Base character loaded: {base_character}");

        Debug.Trace($"Loading character: {info.Scene}");
        var mesh = GDHelper.Instantiate<CharacterMesh>(info.Scene);
        Debug.Trace($"Mesh loaded: {mesh}");

        base_character.SetMesh(mesh);
        Debug.Trace($"Mesh is now set in base character");

        Debug.Indent--;
        return base_character;
    }

    private Character LoadBaseCharacter(CharacterBaseType type)
    {
        Debug.TraceMethod(type);
        Debug.Indent++;

        var path = type switch
        {
            CharacterBaseType.Human => Collection.HumanBase,
            CharacterBaseType.Creature => Collection.CreatureBase,
            _ => ""
        };

        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError($"Found no base character matching type {type}");
        }

        var character = GDHelper.Instantiate<Character>(path);
        character.SetParent(Scene.Current);

        var rng = new RandomNumberGenerator();
        var x = rng.RandfRange(-0.1f, 0.1f);
        var z = rng.RandfRange(-0.1f, 0.1f);
        character.GlobalPosition = new Vector3(x, 0, z);

        Debug.Indent--;
        return character;
    }
}
