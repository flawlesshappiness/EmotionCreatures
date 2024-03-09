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

    private bool automove_enabled;
    private Vector3 automove_direction;

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

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (automove_enabled)
        {
            Move(automove_direction);
        }
    }

    public void Stop()
    {
        Move(Vector3.Zero);
    }

    public void InputMove(Vector2 input)
    {
        InputMove(new Vector3(input.X, 0, input.Y));
    }

    public void InputMove(Vector3 input)
    {
        var direction = MovementLock.IsLocked ? Vector3.Zero : input.Normalized();
        Move(direction * Speed);
    }

    private void Move(Vector3 direction)
    {
        Vector3 velocity = Character.Velocity;

        if (IsControlledByPlayer && CameraBrain.MainCamera != null)
        {
            direction = CameraBrain.MainCamera.Basis * direction;
        }

        // Add the gravity.
        if (!Character.IsOnFloor())
            velocity.Y -= GravityLock.IsLocked ? 0 : Gravity * _time;

        if (direction != Vector3.Zero)
        {
            velocity.X = direction.X;
            velocity.Z = direction.Z;

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

    public void AutoMove(Vector3 direction)
    {
        automove_enabled = true;
        automove_direction = direction;
    }

    public void StopAutoMove()
    {
        automove_enabled = false;
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
