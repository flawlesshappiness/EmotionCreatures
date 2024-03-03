using Godot;

public partial class StaticVirtualCamera : VirtualCamera
{
    public override Transform3D CalculateTransform()
    {
        var transform = FollowTarget != null ? FollowTarget.GlobalTransform : GlobalTransform;
        transform = LookTarget != null ? transform.LookingAt(LookTarget.GlobalPosition) : transform;
        return transform;
    }
}
