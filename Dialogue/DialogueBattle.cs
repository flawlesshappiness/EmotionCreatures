using Godot;

public partial class DialogueBattle : Node
{
    [Export]
    public string DialogueNodeEnd { get; set; }

    public override void _Ready()
    {
        base._Ready();
        DialogueController.Instance.OnDialogueEnded += OnDialogueEnded;
    }

    private void OnDialogueEnded(DialogueEndedArguments args)
    {
        if (args.Node.Id == DialogueNodeEnd)
        {
            BattleController.Instance.StartBattle(ArenaType.MVP);
        }
    }
}
