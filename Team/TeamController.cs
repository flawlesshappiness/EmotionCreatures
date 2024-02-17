using Godot;
using System.Collections.Generic;

public partial class TeamController : Node
{
    public static TeamController Instance => Singleton.TryGet<TeamController>(out var instance) ? instance : Create();
    public static TeamController Create() => Singleton.Create<TeamController>($"Team/{nameof(TeamController)}");

    public override void _Ready()
    {
        base._Ready();
        InitializeDebugActions();
    }

    private void InitializeDebugActions()
    {
        var category = "TEAM";

        Debug.RegisterAction(new DebugAction
        {
            Category = category,
            Text = "Setup default team",
            Action = v => SetupDefaultTeam()
        });
    }

    private void SetupDefaultTeam()
    {
        Save.Game.Team.Creatures = new List<CreatureData>
        {
            new CreatureData { CharacterType = CharacterType.Frog },
            new CreatureData { CharacterType = CharacterType.Frog },
        };

        Game.Save();
    }
}
