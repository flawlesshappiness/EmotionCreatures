using Godot;

public partial class PlayerController : Node
{
    public static PlayerController Instance => Singleton.TryGet<PlayerController>(out var instance) ? instance : Create();

    public static PlayerController Create() =>
        Singleton.CreateSingleton<PlayerController>($"Player/{nameof(PlayerController)}");

    public Character TargetCharacter { get; private set; }

    public void SetTargetCharacter(Character target)
    {
        TargetCharacter = target;
    }
}
