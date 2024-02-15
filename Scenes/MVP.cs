using Godot;

public partial class MVP : Scene
{
    private TopDownCameraFollow camera_follow;
    private Character player;

    protected override void OnInitialize()
    {
        base.OnInitialize();

        BattleController.Instance.OnBattleEnd += OnBattleEnd;

        var spawns = this.GetNodesInChildren<WorldSpawn>();
        spawns.ForEach(x => x.Spawn());

        player = CharacterController.Instance.CreateCharacter(CharacterType.Adventurer);
        camera_follow = this.GetNodeInChildren<TopDownCameraFollow>();
        camera_follow.SetTarget(player);

        PlayerController.Instance.SetTargetCharacter(player);
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
