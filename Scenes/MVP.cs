using Godot;

public partial class MVP : Scene
{
    [NodeName(nameof(BattleArea))]
    public Area3D BattleArea;

    protected override void OnInitialize()
    {
        base.OnInitialize();

        var player = CharacterController.Instance.CreateCharacter(CharacterType.Adventurer);
        var camera = this.GetNodeInChildren<TopDownCameraFollow>();
        camera.SetTarget(player);
        PlayerController.Instance.SetTargetCharacter(player);

        BattleArea.BodyEntered += OnEnterBattleArea;
    }

    private void OnEnterBattleArea(Node3D body)
    {
        var character = body.GetNodeInParents<Character>();

        if (!character.IsPlayer) return;

        BattleController.Instance.StartBattle(ArenaType.MVP);
    }
}
