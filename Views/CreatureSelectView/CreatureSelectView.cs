using Godot;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class CreatureSelectView : View
{
    [NodeType(typeof(CreatureSelectCard))]
    public CreatureSelectCard CreatureSelectCardPrefab;

    [NodeName(nameof(OtherOptions))]
    public MenuOptionsContainer OtherOptions;

    [NodeName(nameof(ValidSelectedCountLabel))]
    public Label ValidSelectedCountLabel;

    [NodeName(nameof(InvalidSelectedCountLabel))]
    public Label InvalidSelectedCountLabel;

    [NodeName(nameof(SFXInvalid))]
    public AudioStreamPlayer SFXInvalid;

    public int MaxSelectCount = 1;

    public TeamData SelectedTeam => CreateSelectedTeam();
    private List<CreatureSelectCard> cards = new();

    public int CountSelected => cards.Count(x => x.Selected);
    public bool ValidSelection => CountSelected > 0 && CountSelected <= MaxSelectCount;

    private bool pressed_confirm;

    public override void _Ready()
    {
        base._Ready();
        Clear();

        OtherOptions.Clear();
        OtherOptions.CreateOption("Confirm", Confirm);

        OnSelectionChanged();
    }

    protected override void OnShow()
    {
        base.OnShow();
        PlayerInput.Instance.MouseVisibleLock.AddLock(nameof(CreatureSelectView));
        PlayerInput.Instance.InputLock.AddLock(nameof(CreatureSelectView));
    }

    protected override void OnHide()
    {
        base.OnHide();
        PlayerInput.Instance.MouseVisibleLock.RemoveLock(nameof(CreatureSelectView));
        PlayerInput.Instance.InputLock.RemoveLock(nameof(CreatureSelectView));
    }

    private void Confirm()
    {
        if (!ValidSelection)
        {
            SFXInvalid.Play();
            return;
        }

        pressed_confirm = true;
        Hide();
    }

    private void Clear()
    {
        CreatureSelectCardPrefab.Visible = false;

        foreach (var card in cards)
        {
            card.QueueFree();
        }

        cards.Clear();
    }

    private void LoadTeam(TeamData data)
    {
        Clear();
        Debug.TraceMethod(data);
        foreach (var creature in data.Creatures)
        {
            CreateCard(creature);
        }

        cards.First().Button.GrabFocus();
    }

    private CreatureSelectCard CreateCard(CreatureData data)
    {
        Debug.TraceMethod(data);
        var card = CreatureSelectCardPrefab.Duplicate() as CreatureSelectCard;
        card.SetParent(CreatureSelectCardPrefab.GetParent());
        card.Show();
        card.LoadCreature(data);
        card.OnCardSelected += _ => OnSelectionChanged();
        card.OnCardDeselected += _ => OnSelectionChanged();
        cards.Add(card);
        return card;
    }

    public Coroutine WaitForPlayerToPickTeam(TeamData data, int count_max)
    {
        MaxSelectCount = count_max;
        pressed_confirm = false;
        LoadTeam(data);
        OnSelectionChanged();

        return Coroutine.Start(Cr);
        IEnumerator Cr()
        {
            while (!pressed_confirm)
            {
                yield return null;
            }

            Debug.Log("PRESSED CONFIRM");
        }
    }

    private TeamData CreateSelectedTeam() => new TeamData
    {
        Creatures = cards
            .Where(x => x.Selected)
            .Select(x => x.CreatureData)
            .ToList()
    };

    private void OnSelectionChanged()
    {
        var count = CountSelected;
        ValidSelectedCountLabel.Text = $"{count}/{MaxSelectCount}";
        InvalidSelectedCountLabel.Text = $"{count}/{MaxSelectCount}";

        var valid = ValidSelection;
        ValidSelectedCountLabel.Visible = valid;
        InvalidSelectedCountLabel.Visible = !valid;
    }
}
