using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class MenuOptionsContainer : ControlScript
{
    [NodeType(typeof(MenuOption))]
    public MenuOption OptionPrefab;

    [NodeName(nameof(SFXSelect))]
    public AudioStreamPlayer SFXSelect;

    private List<Option> options = new();

    private Option selected;

    public bool HasOptions => options.Count > 0;
    public bool HasSelected => selected != null;

    public Action<int> OnOptionSelected;

    private class Option
    {
        public MenuOption Element { get; set; }
        public Action OnSubmit { get; set; }
    }

    public override void _Ready()
    {
        base._Ready();
        OptionPrefab.Hide();
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        PressUp();
        PressDown();
        PressSubmit();
    }

    public void Clear()
    {
        options.ForEach(x => x.Element.QueueFree());
        options.Clear();
        selected = null;
    }

    public void CreateDialogueOptions(List<DialogueNodeOption> node_options)
    {
        Clear();

        node_options.ForEach(x => CreateOption(x.Text, () => DialogueController.Instance.SetDialogueNode(x.Next)));

        SetSelected(0);
    }

    public void CreateOption(string text, Action onSubmit)
    {
        options.Add(new Option
        {
            Element = CreateElement(text, onSubmit),
            OnSubmit = onSubmit
        });
    }

    private MenuOption CreateElement(string text, Action onSubmit)
    {
        var option = OptionPrefab.Duplicate() as MenuOption;
        option.SetParent(OptionPrefab.GetParent());
        option.Text = text;
        option.Button.Pressed += onSubmit;
        option.Button.MouseEntered += () => Hover(option);
        option.Show();
        return option;
    }

    public void SetSelected(int idx)
    {
        if (options.Count == 0) return;
        var next_option = options.GetClamped(idx);
        var previous = selected;

        if (selected == next_option)
        {
            return;
        }

        if (selected != null)
        {
            selected.Element.ShowArrow = false;
            selected = null;
        }

        selected = next_option;

        if (selected != null)
        {
            selected.Element.ShowArrow = true;
            OnOptionSelected?.Invoke(idx);

            if (previous != null)
            {
                SFXSelect.Play();
            }
        }
    }

    public void AdjustSelected(int adjust)
    {
        var idx = selected == null ? 0 : options.IndexOf(selected);
        SetSelected(idx + adjust);
    }

    public void NextOption() => AdjustSelected(1);

    public void PreviousOption() => AdjustSelected(-1);

    public void Submit()
    {
        if (selected == null) return;
        selected.OnSubmit?.Invoke();
    }

    private void Hover(MenuOption element)
    {
        var option = options.FirstOrDefault(x => x.Element == element);
        var index = options.IndexOf(option);
        SetSelected(index);
    }

    private void PressUp()
    {
        if (!HasOptions) return;
        if (Input.IsActionJustPressed(PlayerControls.Forward))
        {
            if (!IsVisibleInTree()) return;
            PreviousOption();
        }
    }

    private void PressDown()
    {
        if (!HasOptions) return;
        if (Input.IsActionJustPressed(PlayerControls.Back))
        {
            if (!IsVisibleInTree()) return;
            NextOption();
        }
    }

    private void PressSubmit()
    {
        if (!HasOptions) return;
        if (selected == null) return;
        if (Input.IsActionJustPressed(PlayerControls.Submit))
        {
            if (!IsVisibleInTree()) return;
            Submit();
        }
    }
}
