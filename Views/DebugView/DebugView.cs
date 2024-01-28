using Godot;
using Godot.Collections;

public partial class DebugView : View
{
    [NodeName("Main")]
    public Control Main;

    [NodeName("Content")]
    public Control Content;

    [NodeName("ButtonPrefab")]
    public Button ButtonPrefab;

    [NodeName("CategoryPrefab")]
    public Label CategoryPrefab;

    [NodeName("ContentSearch")]
    public DebugContentSearch ContentSearch;

    private Dictionary<string, Label> _categories = new();

    public override void _Ready()
    {
        base._Ready();
        Visible = true;
        ButtonPrefab.Visible = false;
        CategoryPrefab.Visible = false;
        HideContent();
        SetVisible(false);

        CreateActionButtons();
        Debug.OnActionAdded += CreateAction;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (@event is InputEventKey keyEvent && keyEvent.Pressed)
        {
            if (Input.IsActionJustPressed("debug"))
            {
                ToggleVisible();
            }
        }
    }

    public void SetVisible(bool visible)
    {
        Main.Visible = visible;

        var parent = GetParent();
        var lock_name = nameof(DebugView);
        if (visible)
        {
            PlayerInput.Instance.MouseVisibleLock.AddLock(lock_name);
            //Player.InteractLock.AddLock(lock_name);
            Scene.PauseLock.AddLock(lock_name);

            var idx_max = parent.GetChildCount() - 1;
            parent.MoveChild(this, idx_max);
        }
        else
        {
            PlayerInput.Instance.MouseVisibleLock.RemoveLock(lock_name);
            //Player.InteractLock.RemoveLock(lock_name);
            Scene.PauseLock.RemoveLock(lock_name);

            parent.MoveChild(this, 0);

            HideContent();
        }
    }

    public void HideContent()
    {
        Content.Visible = false;
        ContentSearch.Visible = false;
    }

    private void ToggleVisible() =>
        SetVisible(!Main.Visible);

    private void CreateActionButtons()
    {
        foreach (var action in Debug.RegisteredActions)
        {
            CreateAction(action);
        }
    }

    private Button CreateActionButton()
    {
        var instance = ButtonPrefab.Duplicate() as Button;
        instance.SetParent(ButtonPrefab.GetParent());
        instance.Visible = true;
        return instance;
    }

    private void CreateAction(DebugAction debug_action)
    {
        var button = CreateActionButton();
        button.Text = debug_action.Text;
        button.Pressed += () => debug_action.Action(this);

        TryCreateCategory(debug_action);
        OrderActionButton(button, debug_action);
    }

    private void TryCreateCategory(DebugAction debug_action)
    {
        if (!_categories.ContainsKey(debug_action.Category))
        {
            CreateActionLabel(debug_action.Category);
        }
    }

    private void OrderActionButton(Button button, DebugAction debug_action)
    {
        var label = _categories[debug_action.Category];
        button.GetParent().MoveChild(button, label.GetIndex() + 1);
    }

    private Label CreateActionLabel(string text)
    {
        var instance = CategoryPrefab.Duplicate() as Label;
        instance.SetParent(CategoryPrefab.GetParent());
        instance.Text = text;
        instance.Visible = true;
        _categories.Add(text, instance);
        return instance;
    }
}
