using System.Collections.Generic;

public partial class BattleMoveControl : ControlScript
{
    [NodeName(nameof(MoveControlNorth))]
    public MoveControl MoveControlNorth;

    [NodeName(nameof(MoveControlEast))]
    public MoveControl MoveControlEast;

    [NodeName(nameof(MoveControlSouth))]
    public MoveControl MoveControlSouth;

    [NodeName(nameof(MoveControlWest))]
    public MoveControl MoveControlWest;

    private List<MoveControl> MoveControls;

    private CreatureCharacter Creature { get; set; }

    public override void _Ready()
    {
        base._Ready();
        MoveControls = new() { MoveControlNorth, MoveControlEast, MoveControlSouth, MoveControlWest };

        for (int i = 0; i < MoveControls.Count; i++)
        {
            var idx_button = i;
            var control = MoveControls.Get(i);
            control.FocusMode = FocusModeEnum.None;
        }

        PlayerInput.Instance.MoveNorth.OnReleased += () => PressMoveButton(0);
        PlayerInput.Instance.MoveEast.OnReleased += () => PressMoveButton(1);
        PlayerInput.Instance.MoveSouth.OnReleased += () => PressMoveButton(2);
        PlayerInput.Instance.MoveWest.OnReleased += () => PressMoveButton(3);

        BattleController.Instance.OnPlayerTargetCreatureChanged += OnCreatureChanged;
    }

    private void OnCreatureChanged(CreatureCharacter creature)
    {
        Creature = creature;
        LoadMoves();
    }

    public void LoadMoves()
    {
        Debug.TraceMethod();
        Debug.Indent++;

        if (Creature == null)
        {
            Debug.LogError("TargetCharacter is not a creature");
            return;
        }

        for (int i = 0; i < MoveControls.Count; i++)
        {
            var control = MoveControls.Get(i);
            var move = Creature.Moves.Moves.Get(i);
            control.Text = move?.Info?.Name ?? "";
            control.Deselect();

            if (move != null)
            {
                control.SetMove(move);
                move.OnSelected += () => OnMoveSelectedChanged(move, control, true);
                move.OnDeselected += () => OnMoveSelectedChanged(move, control, false);
            }
        }

        var selected_index = Creature.Moves.SelectedMoveIndex;
        MoveControls[selected_index].Select();

        Debug.Indent--;
    }

    private void PressMoveButton(int i)
    {
        Debug.TraceMethod(i);
        Debug.Indent++;

        if (Creature == null)
        {
            Debug.LogError("TargetCharacter is not a creature");
            return;
        }

        Creature.Moves.SelectMove(i);
        Debug.Indent--;
    }

    private void OnMoveSelectedChanged(CreatureMove move, MoveControl control, bool selected)
    {
        if (selected)
        {
            control.Select();
        }
        else
        {
            control.Deselect();
        }
    }
}
