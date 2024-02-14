using Godot;

public partial class HumanCharacter : Character
{
    [NodeName(nameof(RayCast))]
    public RayCast3D RayCast;

    public override void _Ready()
    {
        base._Ready();
        PlayerInput.Instance.Submit.OnPressed += PressInteract;
    }

    private void PressInteract()
    {
        if (!RayCast.IsColliding()) return;

        var collider = RayCast.GetCollider() as Node3D;
    }
}
