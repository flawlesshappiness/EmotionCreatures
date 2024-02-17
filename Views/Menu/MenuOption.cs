using Godot;
using System;

public partial class MenuOption : ControlScript
{
    [NodeName(nameof(Arrow))]
    public Control Arrow;

    [NodeName(nameof(Label))]
    public Label Label;

    [NodeName(nameof(Button))]
    public TextureButton Button;

    [NodeName(nameof(SFXSelect))]
    public AudioStreamPlayer SFXSelect;

    public Action OnSelected, OnDeselected;

    public string Text { get => Label.Text; set => Label.Text = value; }

    public bool ShowArrow { get => Arrow.Visible; set => Arrow.Visible = value; }

    private static MenuOption selected;
    private bool HasSelected => IsInstanceValid(selected) && selected != null && selected.IsVisibleInTree();

    public override void _Ready()
    {
        base._Ready();
        Deselect();
        OnSelected += GrabFocus;
        Button.MouseEntered += GrabFocus;
        Button.FocusEntered += Select;
        Button.FocusExited += Deselect;
    }

    public new void GrabFocus()
    {
        Button.GrabFocus();
    }

    private void Select()
    {
        ShowArrow = true;
        OnSelected?.Invoke();

        if (HasSelected)
        {
            SFXSelect.Play();
        }

        selected = this;
    }

    private void Deselect()
    {
        ShowArrow = false;
        OnDeselected?.Invoke();
    }
}
