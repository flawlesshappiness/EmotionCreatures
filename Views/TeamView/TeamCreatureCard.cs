using Godot;

public partial class TeamCreatureCard : ControlScript
{
    [NodeName(nameof(NameLabel))]
    public Label NameLabel;

    [NodeName(nameof(CoreOption))]
    public MenuOption CoreOption;

    [NodeName(nameof(CoreLabel))]
    public Label CoreLabel;

    [NodeName(nameof(MovesOption))]
    public MenuOption MovesOption;

    [NodeName(nameof(Origin))]
    public Node3D Origin;

    private CreatureCharacter creature;

    public override void _Ready()
    {
        base._Ready();
        Clear();
        CoreOption.Text = "Core level: ";
        MovesOption.Text = "Moves";
    }

    public void Clear()
    {
        ClearCreature();
        NameLabel.Text = string.Empty;
        CoreLabel.Text = "1";
    }

    public void SetData(CreatureData data)
    {
        if (data == null)
        {
            Clear();
        }

        var info = CreatureController.Instance.GetInfo(data.CharacterType);

        LoadCharacter(data);

        NameLabel.Text = info.Name;
        CoreLabel.Text = $"{data.Core?.Level ?? 1}";
    }

    private void ClearCreature()
    {
        if (creature != null)
        {
            creature.QueueFree();
            creature = null;
        }
    }

    private void LoadCharacter(CreatureData data)
    {
        ClearCreature();
        creature = CreatureController.Instance.CreateCreature(data);
        creature.SetParent(Origin);
        creature.GlobalPosition = Origin.GlobalPosition;
        creature.GlobalRotation = Origin.GlobalRotation;
        creature.Movement.GravityLock.AddLock(nameof(TeamCreatureCard));
    }
}
