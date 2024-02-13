using Godot;

public partial class MVP : Scene
{
    [NodeName(nameof(BattleArea))]
    public Area3D BattleArea;

    private TopDownCameraFollow camera_follow;
    private Character player;

    protected override void OnInitialize()
    {
        base.OnInitialize();

        BattleController.Instance.OnBattleEnd += OnBattleEnd;

        player = CharacterController.Instance.CreateCharacter(CharacterType.Adventurer);
        camera_follow = this.GetNodeInChildren<TopDownCameraFollow>();
        camera_follow.SetTarget(player);

        PlayerController.Instance.SetTargetCharacter(player);

        BattleArea.BodyEntered += OnEnterBattleArea;

        DialogueController.Instance.SetDialogueNode("##TEST_001##");
    }

    private void OnEnterBattleArea(Node3D body)
    {
        var character = body.GetNodeInParents<Character>();

        if (!character.IsPlayer) return;

        BattleController.Instance.StartBattle(ArenaType.MVP);
    }

    private void OnBattleEnd()
    {
        camera_follow.SetTarget(player);
        PlayerController.Instance.SetTargetCharacter(player);
        player.GlobalPosition = Vector3.Zero;
    }
}
