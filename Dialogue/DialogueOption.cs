using Godot;

public partial class DialogueOption : ControlScript
{
    [NodeName(nameof(Arrow))]
    public Control Arrow;

    [NodeName(nameof(Label))]
    public Label Label;

    [NodeName(nameof(Button))]
    public TextureButton Button;

    public string Text { get => Label.Text; set => Label.Text = value; }

    public bool ShowArrow { get => Arrow.Visible; set => Arrow.Visible = value; }

    public override void _Ready()
    {
        base._Ready();
        ShowArrow = false;
    }
}
