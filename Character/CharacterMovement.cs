using Godot;
using System;

public partial class CharacterMovement : Node
{
    private const float Speed = 5.0f;
    private const float RotationSpeed = 20f;

    private bool _init;
    private bool _moving;

    public MultiLock MovementLock { get; private set; } = new MultiLock();

    public float Gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    public Action OnMoveStart;
    public Action OnMoveEnd;

    public CharacterBody3D Body { get; private set; }

    public bool IsMoving => _moving;

    public void SetBody(CharacterBody3D body)
    {
        Body = body;
    }

    public void Move(Vector2 input, float delta)
    {
        Vector3 velocity = Body.Velocity;
        Vector3 direction = MovementLock.IsLocked ? Vector3.Zero : (new Vector3(input.X, 0, input.Y)).Normalized();

        // Add the gravity.
        if (!Body.IsOnFloor())
            velocity.Y -= Gravity * (float)delta;

        if (direction != Vector3.Zero)
        {
            velocity.X = direction.X * Speed;
            velocity.Z = direction.Z * Speed;

            var ry = Mathf.LerpAngle(Body.Rotation.Y, Mathf.Atan2(velocity.X, velocity.Z), delta * RotationSpeed);
            Body.Rotation = new Vector3(0, ry, 0);
        }
        else
        {
            velocity.X = Mathf.MoveToward(Body.Velocity.X, 0, Speed);
            velocity.Z = Mathf.MoveToward(Body.Velocity.Z, 0, Speed);
        }

        Body.Velocity = velocity;
        Body.MoveAndSlide();

        if (MovementLock.IsFree)
        {
            var hasVelocity = direction != Vector3.Zero;
            if (_moving != hasVelocity)
            {
                _moving = !_moving;
                if (_moving)
                {
                    OnMoveStart?.Invoke();
                }
                else
                {
                    OnMoveEnd?.Invoke();
                }
            }
        }
        else
        {
            _moving = false;
        }
    }
}
