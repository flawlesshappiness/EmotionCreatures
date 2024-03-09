using Godot;

public class AnimationEvent
{
    public AnimationArgs Animation { get; set; }
    public HurtboxArgs Hurtbox { get; set; }
    public MovementArgs Movement { get; set; }
    public ProjectileArgs Projectile { get; set; }

    public class AnimationArgs
    {
    }

    public class HurtboxArgs
    {
        public bool Enabled { get; set; }
    }

    public class MovementArgs
    {
        public bool PlayerInputEnabled { get; set; }
        public bool AutoMoveEnabled { get; set; }
        public Vector3 Direction { get; set; }
        public float Speed { get; set; }
    }

    public class ProjectileArgs
    {
    }
}