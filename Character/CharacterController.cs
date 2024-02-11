using Godot;
using System.Linq;

public partial class CharacterController : Node
{
    public static CharacterController Instance => Singleton.TryGet<CharacterController>(out var instance) ? instance : Create();

    public static CharacterController Create() =>
        Singleton.CreateSingleton<CharacterController>($"Character/{nameof(CharacterController)}");

    private CharacterInfoCollection _collection;
    private CharacterInfoCollection Collection => _collection ?? (_collection = LoadCollection());
    private CharacterInfoCollection LoadCollection() => GD.Load<CharacterInfoCollection>(ResourcePaths.Instance.Collection.CharacterInfoCollection);

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
        var character = Singleton.LoadInstance<Node3D>(info.Scene);
        Debug.Trace($"Character loaded: {character}");

        var animation = base_character.GetNodeInChildren<AnimationController>();
        Debug.Trace($"Animation controller found: {animation}");

        animation.SetModel(character);
        Debug.Trace($"Character is now parented to AnimationController");

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

        var character = Singleton.LoadInstance<Character>(path);

        Debug.Indent--;
        return character;
    }
}
