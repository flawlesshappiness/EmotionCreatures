using Godot;

public partial class VirtualCamera : Node3DScript
{
    [Export(PropertyHint.NodeType, "Node3D")]
    public Node3D FollowTarget { get; set; }

    [Export(PropertyHint.NodeType, "Node3D")]
    public Node3D LookTarget { get; set; }

    private float zoom = 1f;
    private float angle_x, angle_y;

    private Ring RingUpper = new Ring(0.3f, 0.5f);
    private Ring RingMiddle = new Ring(0.6f, 0.0f);
    private Ring RingLower = new Ring(0.4f, -0.5f);

    private Ring CurrentUpperRing => angle_y > 0.5f ? RingUpper : RingMiddle;
    private Ring CurrentLowerRing => angle_y > 0.5f ? RingMiddle : RingLower;
    private float Ty => angle_y > 0.5f ? (angle_y - 0.5f) / 0.5f : angle_y * 2f;

    private class Ring
    {
        public float Height { get; set; }
        public float Radius { get; set; }

        public Ring(float radius, float height)
        {
            Height = height;
            Radius = radius;
        }
    }

    public override void _Ready()
    {
        base._Ready();
        PlayerInput.Instance.LookDirection.OnHeld += OnDirectionHeld;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (Input.MouseMode != Input.MouseModeEnum.Captured)
            return;

        if (@event is InputEventMouseMotion)
        {
            var input = @event as InputEventMouseMotion;
            var x = input.Relative.X * 0.1f;
            var y = input.Relative.Y * 0.001f;
            angle_x = (angle_x + x) % 360;
            angle_y = Mathf.Clamp((angle_y + y), 0f, 1f);
        }

        if (@event is InputEventMouseButton)
        {
            var input = @event as InputEventMouseButton;
            if (input.ButtonIndex == MouseButton.WheelUp)
            {
                AdjustZoom(-1);
            }
            else if (input.ButtonIndex == MouseButton.WheelDown)
            {
                AdjustZoom(1);
            }
        }
    }

    private void OnDirectionHeld(Vector2 input)
    {
        var x = input.X * 8f;
        var y = input.Y * 0.05f;
        angle_x = (angle_x + x) % 360;
        angle_y = Mathf.Clamp((angle_y + y), 0f, 1f);
    }

    private void AdjustZoom(int sign)
    {
        zoom = Mathf.Clamp(zoom + 0.1f * sign, 1f, 5f);
    }

    public Transform3D CalculateTransform()
    {
        var transform = new Transform3D();
        transform = TransformPosition(transform);
        transform = TransformRotation(transform);
        return transform;
    }

    private Transform3D TransformRotation(Transform3D transform)
    {
        if (LookTarget == null) return transform;
        return transform.LookingAt(LookTarget.GlobalPosition, Vector3.Up);
    }

    private Transform3D TransformPosition(Transform3D transform)
    {
        if (FollowTarget == null) return transform;

        var upper = CurrentUpperRing;
        var lower = CurrentLowerRing;
        var t = Ty;

        var y = Mathf.Lerp(lower.Height, upper.Height, t);
        var r = Mathf.Lerp(lower.Radius, upper.Radius, t);

        var p = MathHelper.CirclePoint(r, angle_x);
        var offset = FollowTarget.GlobalPosition + new Vector3(p.X, y, p.Y) * zoom;
        return transform.Translated(offset);
    }
}
