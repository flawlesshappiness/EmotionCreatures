using Godot;

public partial class CharacterNavigation : NavigationAgent3D
{
    public MeshInstance3D DebugSphere;

    public MultiLock NavigationLock = new MultiLock();

    protected Character Character { get; set; }
    private bool IsReady { get; set; }

    private bool is_at_destination;
    private Node3D _navNode;

    public override void _Ready()
    {
        base._Ready();
    }

    public virtual void Initialize(Character character)
    {
        Character = character;

        DebugSphere = GetNode<MeshInstance3D>("DebugSphere");
        DebugSphere.Visible = false;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (!IsReady)
        {
            IsReady = true;
            return;
        }

        if (NavigationLock.IsLocked) return;

        ProcessMoveTowardsTarget();
    }

    private void ProcessMoveTowardsTarget()
    {
        if (IsNavigationFinished())
        {
            if (!is_at_destination)
            {
                is_at_destination = true;
                Character.Movement.Stop();
            }
            return;
        }

        Vector3 currentAgentPosition = Character.GlobalPosition;
        Vector3 nextPathPosition = GetNextPathPosition();

        var dir = currentAgentPosition.DirectionTo(nextPathPosition);
        //DebugSphere.Visible = true;
        DebugSphere.GlobalPosition = nextPathPosition;
        Character.Movement.Move(dir);
    }

    public void NavigatoTo(Vector3 position)
    {
        is_at_destination = false;
        TargetPosition = position;
    }

    public void NavigatoToRandomPosition(Vector3 center, float radius_max, float? radius_min = null)
    {
        var rnd = new RandomNumberGenerator();
        var x = rnd.RandfRange(-1, 1);
        var z = rnd.RandfRange(-1, 1);
        var dir = new Vector3(x, 0, z).Normalized();
        dir *= radius_min == null ? radius_max : rnd.RandfRange(radius_min.Value, radius_max);
        var position = center + dir;
        NavigatoTo(position);
    }
}
