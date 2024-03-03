using Godot;

public abstract partial class VirtualCamera : Node3DScript
{
    [Export(PropertyHint.NodeType, "Node3D")]
    public Node3D FollowTarget { get; set; }

    [Export(PropertyHint.NodeType, "Node3D")]
    public Node3D LookTarget { get; set; }

    public abstract Transform3D CalculateTransform();

    public void MoveTo(float duration, Curve curve)
    {
        CameraBrain.MainCamera.MoveTo(this, duration, curve);
    }

    public void TeleportTo()
    {
        CameraBrain.MainCamera.TeleportTo(this);
    }
}
