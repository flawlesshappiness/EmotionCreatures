using Godot;
using System;

public partial class VirtualCamera : Node3DScript
{
    [Export(PropertyHint.NodeType, "Node3D")]
    public Node3D FollowTarget { get; set; }

    [Export(PropertyHint.NodeType, "Node3D")]
    public Node3D LookTarget { get; set; }

    [Export(PropertyHint.Range, "0.25, 10")]
    public float ZoomMin { get; set; }

    [Export(PropertyHint.Range, "0.25, 10")]
    public float ZoomMax { get; set; }

    private float zoom_adjust = 0.05f;
    private static float zoom_perc = 0f;
    private float zoom_value = 0.01f;
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

    public override void _Process(double delta)
    {
        base._Process(delta);
        ProcessZoom(delta);
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
            InputLook(x, y);
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
        InputLook(x, y);
    }

    private void InputLook(float x, float y)
    {
        angle_x = (angle_x + x) % 360;
        angle_y = Mathf.Clamp((angle_y + y), 0f, 1f);
    }

    private void SetZoom(float perc)
    {
        zoom_perc = perc;
        zoom_value = Mathf.Lerp(ZoomMin, ZoomMax, zoom_perc);
    }

    private void AdjustZoom(int sign)
    {
        zoom_perc = Mathf.Clamp(zoom_perc + zoom_adjust * sign, 0f, 1f);
    }

    private void ProcessZoom(double delta)
    {
        var start = zoom_value;
        var end = Mathf.Lerp(ZoomMin, ZoomMax, zoom_perc);
        zoom_value = Mathf.Lerp(start, end, 10 * Convert.ToSingle(delta));
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
        var offset = FollowTarget.GlobalPosition + new Vector3(p.X, y, p.Y) * zoom_value;
        return transform.Translated(offset);
    }
}
