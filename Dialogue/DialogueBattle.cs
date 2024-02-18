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
            var battle_args = new StartBattleArgs
            {
                ArenaType = ArenaType.MVP,

                OpponentTeam = new() // TODO
                {
                    Creatures = new()
                    {
                        new()
                        {
                            CharacterType = CharacterType.Frog,
                            Moveset = new()
                            {
                                Moves = new(){ MoveType.Punch, MoveType.Projectile }
                            }
                        }
                    }
                },
            };

            BattleController.Instance.StartBattle(battle_args);
        }
    }
}
