using System.Collections.Generic;

public partial class TeamView : View
{
    [NodeType(typeof(TeamCreatureCard))]
    public TeamCreatureCard CardPrefab;

    [NodeName(nameof(OtherOptions))]
    public MenuOptionsContainer OtherOptions;

    private List<TeamCreatureCard> cards = new();

    public override void _Ready()
    {
        base._Ready();

        CreateCards();
        OtherOptions.CreateOption("Back", Back);

        Debug.Log(TeamController.Instance);
    }

    protected override void OnShow()
    {
        base.OnShow();
        PlayerInput.Instance.MouseVisibleLock.AddLock(nameof(TeamView));
        PlayerInput.Instance.InputLock.AddLock(nameof(TeamView));

        LoadCards();
        OtherOptions.GrabFocus();
    }

    protected override void OnHide()
    {
        base.OnHide();
        PlayerInput.Instance.MouseVisibleLock.RemoveLock(nameof(TeamView));
        PlayerInput.Instance.InputLock.RemoveLock(nameof(TeamView));
    }

    private void Back()
    {
        Hide();
        Show<GameMenuView>();
    }

    private void CreateCards()
    {
        CardPrefab.Hide();

        for (int i = 0; i < Constants.MAX_TEAM_SIZE; i++)
        {
            CreateCard();
        }
    }

    private TeamCreatureCard CreateCard()
    {
        var card = CardPrefab.Duplicate() as TeamCreatureCard;
        card.SetParent(CardPrefab.GetParent());
        card.Show();
        cards.Add(card);
        return card;
    }

    private void LoadCards()
    {
        var team = Save.Game.Team;
        for (int i = 0; i < cards.Count; i++)
        {
            var card = cards.Get(i);
            if (card == null) continue;

            var data = team.Creatures.Get(i);
            if (data == null)
            {
                card.Clear();
                continue;
            }

            card.SetData(data);
        }
    }
}
