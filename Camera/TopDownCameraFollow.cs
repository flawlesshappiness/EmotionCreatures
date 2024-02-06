using Godot;
using System;

public partial class TopDownCameraFollow : Camera3D
{
    [Export]
    public float Speed;

    [Export]
    public Vector3 Offset;

    [Export]
    public Vector3 DebugOffset;

    private Node3D _target;

    private Vector3 _offset;
    private bool _zoomed_out;

    public override void _Ready()
    {
        base._Ready();
        _offset = Offset;

        Debug.RegisterAction(new DebugAction
        {
            Category = "Camera",
            Text = "Zoom out",
            Action = _ => ToggleZoomOut()
        });
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (_target == null) return;

        var f = Convert.ToSingle(delta);

        var start = GlobalPosition;
        var end = _target.GlobalPosition + _offset;
        GlobalPosition = Lerp.Vector3(start, end, f * Speed);
    }

    public void SetTarget(Node3D target)
    {
        _target = target;
        GlobalPosition = _target.GlobalPosition + _offset;
    }

    private void ToggleZoomOut()
    {
        _zoomed_out = !_zoomed_out;
        _offset = _zoomed_out ? DebugOffset : Offset;
    }
}
