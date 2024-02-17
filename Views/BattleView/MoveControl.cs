using Godot;

public partial class MoveControl : ControlScript
{
    [NodeType(typeof(TextureProgressBar))]
    public TextureProgressBar ProgressBar;

    [NodeName(nameof(NameLabel))]
    public Label NameLabel;

    [NodeName(nameof(SelectedTexture))]
    public TextureRect SelectedTexture;

    public string Text { get => NameLabel.Text; set => NameLabel.Text = value; }

    public CreatureMove Move { get; private set; }

    public override void _Ready()
    {
        base._Ready();
        Deselect();
    }

    public void SetMove(CreatureMove move)
    {
        Move = move;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        ProcessProgress();
    }

    private void ProcessProgress()
    {
        if (Move == null)
        {
            ProgressBar.Value = ProgressBar.MinValue;
        }
        else if (Move.IsOnCooldown)
        {
            var t = MathHelper.Percentage(TimeHelper.CurrentTime, Move.TimeCooldownStart, Move.TimeCooldownEnd);
            ProgressBar.Value = MathHelper.Percentage(t, ProgressBar.MinValue, ProgressBar.MaxValue);
        }
        else
        {
            ProgressBar.Value = ProgressBar.MaxValue;
        }
    }

    public void Select()
    {
        SelectedTexture.Show();
    }

    public void Deselect()
    {
        SelectedTexture.Hide();
    }
}
