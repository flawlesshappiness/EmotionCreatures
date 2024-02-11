using Godot;

public partial class PlayerController : Node
{
    public static PlayerController Instance => Singleton.TryGet<PlayerController>(out var instance) ? instance : Create();

    public static PlayerController Create() =>
        Singleton.CreateSingleton<PlayerController>($"Player/{nameof(PlayerController)}");

    public Character TargetCharacter { get; private set; }

    public void SetTargetCharacter(Character target)
    {
        Debug.LogMethod(target);
        Debug.Indent++;
        if (TargetCharacter != null)
        {
            TargetCharacter.EndTarget();
            TargetCharacter = null;
        }

        TargetCharacter = target;

        if (TargetCharacter != null)
        {
            TargetCharacter.BeginTarget();
        }
        Debug.Indent--;
    }

    public void RemoveTargetCharacter()
    {
        Debug.LogMethod();
        Debug.Indent++;
        SetTargetCharacter(null);
        Debug.Indent--;
    }
}
