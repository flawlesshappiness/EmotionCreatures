using Godot;

public partial class HumanCharacter : Character
{
    [NodeName(nameof(RayCast))]
    public RayCast3D RayCast;

    public override void _Ready()
    {
        base._Ready();
        PlayerInput.Instance.Submit.OnPressed += PressInteract;

        Movement.Speed = 4.0f;
    }

    private void PressInteract()
    {
        if (!IsPlayer) return;

        Debug.TraceMethod();
        if (!RayCast.IsColliding()) return;

        var collider = RayCast.GetCollider() as Node3D;
        Debug.Trace("Collider: " + collider);
        var interactable = collider.GetNodeInParents<Interactable>();
        Debug.Trace("Interactable: " + collider);
        if (interactable == null) return;

        interactable.TryInteract(null);
    }
}
