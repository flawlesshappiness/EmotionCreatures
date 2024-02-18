using Godot;
using System;

public partial class Projectile : CharacterBody3D
{
    [NodeType(typeof(CollisionShape3D))]
    public CollisionShape3D Collider;

    public MoveInfo Info { get; set; }
    public CreatureCharacter Sender { get; set; }

    private float Speed { get; set; }
    private Vector3 Direction { get; set; }

    private bool _fired;
    private float _time;

    public override void _Ready()
    {
        base._Ready();
        NodeScript.FindNodesFromAttribute(this, GetType());
    }

    public void Fire(Vector3 direction)
    {
        Speed = Info.ProjectileSpeed;
        Direction = direction;
        _fired = true;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        _time = Convert.ToSingle(delta);

        if (_fired)
        {
            var input = new Vector2(Direction.X, Direction.Z);
            Move(input);
        }
    }

    public void Move(Vector2 input)
    {
        Vector3 velocity = Velocity;
        Vector3 direction = (new Vector3(input.X, 0, input.Y)).Normalized();

        if (direction != Vector3.Zero)
        {
            velocity.X = direction.X * Speed;
            velocity.Z = direction.Z * Speed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
            velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
        }

        Velocity = velocity;
        MoveAndSlide();

        var ccount = GetSlideCollisionCount();
        var valid_hit = IsOnWall();
        for (int i = 0; i < ccount; i++)
        {
            var collision = GetSlideCollision(i);
            var collider = collision.GetCollider() as Node3D;
            if (collider == null) continue;
            var creature = collider.GetNodeInParents<CreatureCharacter>();
            if (creature == null) continue;
            if (creature == Sender) continue;

            creature.Damage(Info.Damage);
            valid_hit = true;
            break;
        }

        if (valid_hit)
        {
            QueueFree();
        }
    }
}
