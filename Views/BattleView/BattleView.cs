public partial class BattleView : View
{
    [NodeType(typeof(BattleMoveControl))]
    public BattleMoveControl MoveControl;

    protected override void OnShow()
    {
        base.OnShow();
        MoveControl.LoadMoves();
    }
}
