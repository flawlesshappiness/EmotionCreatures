using Godot;
using System.Collections;

public partial class CameraBrain : Node3DScript
{
    [NodeName(nameof(Raycast))]
    public RayCast3D Raycast;

    [NodeType(typeof(Camera3D))]
    public Camera3D Camera;

    public new Basis Basis => Camera.Basis;

    public static CameraBrain MainCamera { get; private set; }

    public VirtualCamera CurrentVCam { get; private set; }

    private Coroutine cr_move_to;

    public override void _Ready()
    {
        base._Ready();
        NodeScript.FindNodesFromAttribute(this, GetType());

        if (MainCamera == null)
        {
            MainCamera = this;
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (CurrentVCam == null) return;
        if (cr_move_to != null) return;

        var transform = CurrentVCam.CalculateTransform();
        Camera.GlobalTransform = transform;

        ProcessRaycastPoint();
        //ProcessRaycastGround();
    }

    private void ProcessRaycastPoint()
    {
        var extra_length = 0.01f;
        var dir_to_camera = Camera.GlobalPosition - Raycast.GlobalPosition;
        Raycast.GlobalPosition = CurrentVCam.FollowTarget.GlobalPosition;
        Raycast.TargetPosition = dir_to_camera + dir_to_camera.Normalized() * extra_length;
        if (Raycast.IsColliding())
        {
            var point = Raycast.GetCollisionPoint();
            var dir_to_point = point - Raycast.GlobalPosition;
            var dir_to_point_extended = dir_to_point - dir_to_point.Normalized() * extra_length;
            if (dir_to_point_extended.Length() < dir_to_camera.Length())
            {
                var dir = point - Camera.GlobalPosition;
                var offset = dir.Normalized() * extra_length;
                Camera.Transform = Camera.Transform.Translated(dir + offset);
            }
        }
    }

    private void ProcessRaycastGround()
    {
        Raycast.GlobalPosition = Camera.GlobalPosition;
        Raycast.TargetPosition = new Vector3(0, -0.1f, 0);
        if (Raycast.IsColliding())
        {
            var point = Raycast.GetCollisionPoint() + new Vector3(0, 0.1f, 0);
            var dir = point - Camera.GlobalPosition;
            Camera.Transform = Camera.Transform.Translated(dir);
        }
    }

    public void TeleportTo(VirtualCamera vcam)
    {
        CurrentVCam = vcam;
        Camera.GlobalTransform = CurrentVCam.CalculateTransform();
    }

    public void MoveTo(VirtualCamera vcam, float duration, Curve curve)
    {
        CurrentVCam = vcam;
        Coroutine.Stop(cr_move_to);
        cr_move_to = Coroutine.Start(Cr);
        IEnumerator Cr()
        {
            var start = Camera.GlobalTransform;
            yield return LerpEnumerator.Lerp01(duration, f =>
            {
                var end = vcam.CalculateTransform();
                Camera.GlobalTransform = Lerp.Transform(start, end, curve.Evaluate(f));
            });
            cr_move_to = null;
        }
    }
}
