using Godot;
using System;

public partial class CharacterMovement : Node
{
    private const float RotationSpeed = 20f;

    public float Speed { get; set; }

    private bool _init;
    private bool _moving;
    private float _time;

    public Action OnMoveStart;
    public Action OnMoveEnd;

    private Node3D look_at_target;

    public MultiLock GravityLock { get; private set; } = new MultiLock();
    public MultiLock MovementLock { get; private set; } = new MultiLock();

    public float Gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    public Character Character { get; private set; }

    public bool IsMoving => _moving;

    private bool HasActiveAI => Character.AI?.Active ?? false;
    private bool IsControlledByPlayer => Character.IsPlayer && !HasActiveAI;

    public void Initialize(Character character)
    {
        Character = character;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        _time = Convert.ToSingle(delta);

        if (look_at_target != null)
        {
            RotateTowards(look_at_target.GlobalPosition);
        }
    }

    public void Stop()
    {
        Move(Vector2.Zero);
    }

    public void Move(Vector3 input)
    {
        Move(new Vector2(input.X, input.Z));
    }

    public void Move(Vector2 input)
    {
        Vector3 velocity = Character.Velocity;
        Vector3 direction = MovementLock.IsLocked ? Vector3.Zero : (new Vector3(input.X, 0, input.Y)).Normalized();

        if (IsControlledByPlayer && CameraBrain.MainCamera != null)
        {
            direction = CameraBrain.MainCamera.Basis * direction;
        }

        // Add the gravity.
        if (!Character.IsOnFloor())
            velocity.Y -= GravityLock.IsLocked ? 0 : Gravity * _time;

        if (direction != Vector3.Zero)
        {
            velocity.X = direction.X * Speed;
            velocity.Z = direction.Z * Speed;

            Rotate(velocity);
        }
        else
        {
            velocity.X = Mathf.MoveToward(Character.Velocity.X, 0, Speed);
            velocity.Z = Mathf.MoveToward(Character.Velocity.Z, 0, Speed);
        }

        Character.Velocity = velocity;
        Character.MoveAndSlide();

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

    public void Rotate(Vector3 velocity)
    {
        var ry = Mathf.LerpAngle(Character.Rotation.Y, Mathf.Atan2(velocity.X, velocity.Z), RotationSpeed * _time);
        Character.Rotation = new Vector3(0, ry, 0);
    }

    public void RotateTowards(Vector3 position)
    {
        var dir = Character.GlobalPosition.DirectionTo(position);
        Rotate(dir);
    }

    public void StartLookingAt(Node3D node)
    {
        look_at_target = node;
    }

    public void StopLookingAt() => StartLookingAt(null);
}
