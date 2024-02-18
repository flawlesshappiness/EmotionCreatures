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

    [NodeType(typeof(WorldObjectControl))]
    public WorldObjectControl WorldObject;

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
        WorldObject.Clear();
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

        NameLabel.Text = info.Name;
        CoreLabel.Text = $"{data.Core?.Level ?? 1}";
        WorldObject.LoadCreature(data);
    }
}
