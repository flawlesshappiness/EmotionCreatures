using Godot;
using System;
using System.Collections;

public partial class CreatureSelectCard : ControlScript
{
    [NodeName(nameof(HoverTexture))]
    public TextureRect HoverTexture;

    [NodeName(nameof(SelectedTexture))]
    public TextureRect SelectedTexture;

    [NodeName(nameof(NameLabel))]
    public Label NameLabel;

    [NodeName(nameof(Button))]
    public TextureButton Button;

    [NodeName(nameof(WorldObjectControl))]
    public WorldObjectControl WorldObjectControl;

    [NodeName(nameof(SFXHover))]
    public AudioStreamPlayer SFXHover;

    [NodeName(nameof(SFXSelect))]
    public AudioStreamPlayer SFXSelect;

    [NodeName(nameof(SFXDeselect))]
    public AudioStreamPlayer SFXDeselect;

    public CreatureData CreatureData { get; private set; }
    public bool Selected { get; set; }

    public Action<CreatureSelectCard> OnCardSelected;
    public Action<CreatureSelectCard> OnCardDeselected;

    private Coroutine cr_selected;
    private Coroutine cr_hover;

    public override void _Ready()
    {
        base._Ready();

        Button.MouseEntered += MouseEntered;
        Button.FocusEntered += FocusEntered;
        Button.FocusExited += FocusExited;
        Button.Pressed += Pressed;

        SelectedTexture.Scale = Vector2.Zero;
        HoverTexture.Scale = Vector2.Zero;
    }

    public void LoadCreature(CreatureData data)
    {
        CreatureData = data;

        var info = CreatureController.Instance.GetInfo(data.CharacterType);
        Debug.TraceMethod(info.Name);
        NameLabel.Text = info.Name;
        WorldObjectControl.LoadCreature(data);
    }

    new private void MouseEntered()
    {
        Button.GrabFocus();
    }

    new private void FocusEntered()
    {
        SFXHover.Play();
        AnimateFocus(true);
    }

    new private void FocusExited()
    {
        AnimateFocus(false);
    }

    private void Pressed()
    {
        if (Selected)
        {
            Deselect();
        }
        else
        {
            Select();
        }
    }

    private void Select()
    {
        Selected = true;
        AnimateSelect(true);
        SFXSelect.Play();
        OnCardSelected?.Invoke(this);
    }

    private void Deselect()
    {
        Selected = false;
        AnimateSelect(false);
        SFXDeselect.Play();
        OnCardDeselected?.Invoke(this);
    }

    private void AnimateFocus(bool show)
    {
        Coroutine.Stop(cr_hover);
        cr_hover = Coroutine.Start(Cr);
        IEnumerator Cr()
        {
            var start = HoverTexture.Scale;
            var end = show ? Vector2.One : Vector2.Zero;
            var curve = show ? Curves.EaseOutBack : Curves.EaseInBack;
            yield return LerpEnumerator.Lerp01(0.2f, f =>
            {
                HoverTexture.Scale = Lerp.Vector(start, end, curve.Evaluate(f));
            });
        }
    }

    private void AnimateSelect(bool show)
    {
        Coroutine.Stop(cr_selected);
        cr_selected = Coroutine.Start(Cr);
        IEnumerator Cr()
        {
            var start = SelectedTexture.Scale;
            var end = show ? Vector2.One : Vector2.Zero;
            var curve = show ? Curves.EaseOutBack : Curves.EaseInBack;
            yield return LerpEnumerator.Lerp01(0.2f, f =>
            {
                SelectedTexture.Scale = Lerp.Vector(start, end, curve.Evaluate(f));
            });
        }
    }
}
