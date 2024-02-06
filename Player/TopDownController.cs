using Godot;
using System;

public partial class TopDownController : Character
{
    private const float Speed = 5.0f;
    private const float RotationSpeed = 20f;

    private bool _init;
    private bool _moving;

    public float Gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    public Action OnMoveStart;
    public Action OnMoveEnd;

    public override void _Process(double delta)
    {
        ProcessMove(Convert.ToSingle(delta));
    }

    private void ProcessMove(float delta)
    {
        Vector3 velocity = Velocity;
        Vector2 inputDir = Input.GetVector(PlayerControls.Left, PlayerControls.Right, PlayerControls.Forward, PlayerControls.Back);
        Vector3 direction = (new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

        // Add the gravity.
        if (!IsOnFloor())
            velocity.Y -= Gravity * (float)delta;

        if (direction != Vector3.Zero)
        {
            velocity.X = direction.X * Speed;
            velocity.Z = direction.Z * Speed;

            var ry = Mathf.LerpAngle(Rotation.Y, Mathf.Atan2(velocity.X, velocity.Z), delta * RotationSpeed);
            Rotation = new Vector3(0, ry, 0);

            if (!_moving)
            {
                _moving = true;
                OnMoveStart?.Invoke();
            }
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
            velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);

            if (_moving)
            {
                _moving = false;
                OnMoveEnd?.Invoke();
            }
        }

        Velocity = velocity;
        MoveAndSlide();
    }
}
