using Godot;

public partial class BattleView : View
{
    [NodeType(typeof(BattleMoveControl))]
    public BattleMoveControl MoveControl;

    [NodeName(nameof(BGM))]
    public AudioStreamPlayer BGM;

    [NodeName(nameof(AIControl_Value))]
    public Label AIControl_Value;

    public override void _Ready()
    {
        base._Ready();
        BattleController.Instance.OnToggleAI += OnToggleAI;
        BattleController.Instance.OnBattleStart += OnBattleStart;
    }

    protected override void OnShow()
    {
        base.OnShow();
        MoveControl.LoadMoves();

        BGM.Play();
    }

    protected override void OnHide()
    {
        base.OnHide();
        BGM.Stop();
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
