using Godot;

public partial class BattleView : View
{
    [NodeType(typeof(BattleMoveControl))]
    public BattleMoveControl MoveControl;

    [NodeName(nameof(BGM))]
    public AudioStreamPlayer BGM;

    [NodeName(nameof(AIControl_Value))]
    public Label AIControl_Value;

    [NodeName(nameof(TargetCreature_Value))]
    public Label TargetCreature_Value;

    public override void _Ready()
    {
        base._Ready();
        BattleController.Instance.OnToggleAI += OnToggleAI;
        BattleController.Instance.OnBattleStart += OnBattleStart;
        BattleController.Instance.OnPlayerTargetCreatureChanged += OnTargetChanged;
    }

    private void OnTargetChanged(CreatureCharacter creature)
    {
        TargetCreature_Value.Text = creature.Info.Name;
        OnToggleAI();
    }

    protected override void OnShow()
    {
        base.OnShow();
        MoveControl.LoadMoves();
    }

    protected override void OnHide()
    {
        base.OnHide();
    }

    private void OnBattleStart(StartBattleArgs args)
    {
        OnToggleAI();
    }

    private void OnToggleAI()
    {
        var active = BattleController.Instance.TargetPlayerCreature.AI.Active;
        var sActive = active ? "AI" : "Player";
        AIControl_Value.Text = sActive;
    }
}
