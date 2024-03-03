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
        transform = ProcessRaycastPoint(transform);
        Camera.GlobalTransform = transform;
    }

    private Transform3D ProcessRaycastPoint(Transform3D transform)
    {
        var center = CurrentVCam.FollowTarget.GlobalPosition;
        Raycast.GlobalPosition = center;
        var dir_to_origin = transform.Origin - center;
        Raycast.TargetPosition = dir_to_origin;

        if (Raycast.IsColliding())
        {
            var point = Raycast.GetCollisionPoint();
            var dir = point - transform.Origin;
            transform = transform.Translated(dir);
        }

        transform = transform.Translated(-dir_to_origin.Normalized() * 0.1f);
        return transform;
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
