using Godot;
using System.Linq;

public partial class CharacterController : Node
{
    public static CharacterController Instance => Singleton.TryGet<CharacterController>(out var instance) ? instance : Create();

    public static CharacterController Create() =>
        Singleton.Create<CharacterController>($"Character/{nameof(CharacterController)}");

    private CharacterInfoCollection _collection;
    private CharacterInfoCollection Collection => _collection ?? (_collection = LoadCollection());
    private CharacterInfoCollection LoadCollection() => CharacterInfoCollection.Load<CharacterInfoCollection>(ResourcePaths.Instance.Collection.CharacterInfoCollection);

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

        var animation = base_character.GetNodeInChildren<CharacterAnimator>();
        Debug.Trace($"Animation controller found: {animation}");

        animation.SetMesh(mesh);
        Debug.Trace($"Character is now parented to Animator");

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

        Debug.Indent--;
        return character;
    }
}
