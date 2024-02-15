using Godot;

public partial class Interactable : Node3D
{
    public event System.Action OnInteract;
    public event System.Action OnInteractEnd;

    public override void _Ready()
    {
        base._Ready();
    }

    public bool TryInteract(System.Action onInteractEnd)
    {
        OnInteractEnd = onInteractEnd;
        Interact();
        return true;
    }

    protected virtual void Interact()
    {
        Debug.Log($"Interacted with: {GetParent().Name}");
        OnInteract?.Invoke();
    }

    protected virtual void EndInteraction()
    {
        Debug.Log($"Interaction ended");
        OnInteractEnd?.Invoke();
    }
}
