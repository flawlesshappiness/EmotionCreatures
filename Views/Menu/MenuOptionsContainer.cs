using System;
using System.Collections.Generic;
using System.Linq;

public partial class MenuOptionsContainer : ControlScript
{
    [NodeType(typeof(MenuOption))]
    public MenuOption OptionPrefab;

    private List<Option> options = new();

    public bool HasOptions => options.Count > 0;

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

    public void GrabFocus()
    {
        var option = options.FirstOrDefault();
        if (option != null)
        {
            option.Element.Button.GrabFocus();
        }
    }

    public void Clear()
    {
        options.ForEach(x => x.Element.QueueFree());
        options.Clear();
    }

    public void CreateDialogueOptions(List<DialogueNodeOption> node_options)
    {
        Clear();
        node_options.ForEach(x => CreateOption(x.Text, () => DialogueController.Instance.SetDialogueNode(x.Next)));
    }

    public void CreateOption(string text, Action onSubmit)
    {
        var option = new Option
        {
            Element = CreateElement(text, onSubmit),
            OnSubmit = onSubmit
        };

        options.Add(option);
    }

    private MenuOption CreateElement(string text, Action onSubmit)
    {
        var option = OptionPrefab.Duplicate() as MenuOption;
        option.SetParent(OptionPrefab.GetParent());
        option.Text = text;
        option.Button.Pressed += onSubmit;
        option.Show();
        return option;
    }
}
