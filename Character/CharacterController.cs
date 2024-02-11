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
        var info = Collection.Resources.FirstOrDefault(x => x.Type == type);

        if (info == null)
        {
            Debug.LogError($"Found no character with type: {type}");
            return null;
        }

        var character = Singleton.LoadInstance<Character>(info.Scene);
        Debug.Log($"Created character: {character}");

        Debug.Indent--;
        return character;
    }
}
