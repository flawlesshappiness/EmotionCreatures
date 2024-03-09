using Godot;

public partial class PlayerController : Node
{
    public static PlayerController Instance => Singleton.TryGet<PlayerController>(out var instance) ? instance : Create();

    public static PlayerController Create() =>
        Singleton.Create<PlayerController>($"Player/{nameof(PlayerController)}");

    public Character TargetCharacter { get; private set; }

    public override void _Ready()
    {
        base._Ready();
        PlayerInput.Instance.MoveDirection.OnHeld += InputMove;
        PlayerInput.Instance.MoveDirection.OnEnded += _ => InputMove(Vector2.Zero);
    }

    private void InputMove(Vector2 direction)
    {
        if (TargetCharacter == null) return;
        if (!TargetCharacter.CanControl) return;
        TargetCharacter.Movement.PlayerInputMove(direction);
    }

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
