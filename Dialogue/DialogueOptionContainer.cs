using System;
using System.Collections.Generic;
using System.Linq;

public partial class DialogueOptionContainer : ControlScript
{
    [NodeName(nameof(DialogueOption))]
    public DialogueOption DialogueOption;

    private List<Option> options = new();

    private Option selected;

    public bool HasOptions => options.Count > 0;

    private class Option
    {
        public DialogueOption Element { get; set; }
        public Action OnSubmit { get; set; }
    }

    public override void _Ready()
    {
        base._Ready();
        DialogueOption.Hide();
    }

    public void Clear()
    {
        options.ForEach(x => x.Element.QueueFree());
        options.Clear();
    }

    public void CreateOptions(List<DialogueNodeOption> node_options)
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

    private DialogueOption CreateElement(string text, Action onSubmit)
    {
        var option = DialogueOption.Duplicate() as DialogueOption;
        option.SetParent(DialogueOption.GetParent());
        option.Text = text;
        option.Button.Pressed += onSubmit;
        option.Button.MouseEntered += () => Hover(option);
        option.Show();
        return option;
    }

    public void SetSelected(int idx)
    {
        var next_option = options.GetClamped(idx);

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

    private void Hover(DialogueOption element)
    {
        var option = options.FirstOrDefault(x => x.Element == element);
        var index = options.IndexOf(option);
        SetSelected(index);
    }
}
