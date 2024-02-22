using Godot;
using System.Collections;

public partial class CameraBrain : Camera3D
{
    public static CameraBrain MainCamera { get; private set; }

    public VirtualCamera CurrentVCam { get; private set; }

    private Coroutine cr_move_to;

    public override void _Ready()
    {
        base._Ready();

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
        Transform = transform;
    }

    public void TeleportTo(VirtualCamera vcam)
    {
        CurrentVCam = vcam;
        Transform = CurrentVCam.CalculateTransform();
    }

    public void MoveTo(VirtualCamera vcam, float duration, Curve curve)
    {
        CurrentVCam = vcam;
        Coroutine.Stop(cr_move_to);
        cr_move_to = Coroutine.Start(Cr);
        IEnumerator Cr()
        {
            var start = Transform;
            yield return LerpEnumerator.Lerp01(duration, f =>
            {
                var end = vcam.CalculateTransform();
                Transform = Lerp.Transform(start, end, curve.Evaluate(f));
            });
            cr_move_to = null;
        }
    }
}
