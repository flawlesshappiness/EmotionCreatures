using Godot;

public partial class MVP : Scene
{
    [NodeName(nameof(BattleArea))]
    public Area3D BattleArea;

    protected override void OnInitialize()
    {
        base.OnInitialize();

        var camera = this.GetNodeInChildren<TopDownCameraFollow>();
        var target = this.GetNodeInChildren<Character>();
        camera.SetTarget(target);
        PlayerController.Instance.SetTargetCharacter(target);

        BattleArea.BodyEntered += OnEnterBattleArea;
    }

    private void OnEnterBattleArea(Node3D body)
    {
        var character = body.GetNodeInParents<Character>();

        if (!character.IsPlayer) return;

        BattleController.Instance.StartBattle(ArenaType.MVP);
    }
}
