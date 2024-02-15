using Godot;

public partial class InteractableDialogueCharacter : Interactable
{
    [Export]
    public string Id { get; set; }

    [Export]
    public string DefaultNode { get; set; }

    private Character character;
    public Character Character => character ?? (character = this.GetNodeInParents<Character>());

    protected override void Interact()
    {
        base.Interact();

        Debug.Log($"InteractableDialogueCharacter.Interact: {Name}");
        Debug.Indent++;

        Character.Movement.StartLookingAt(PlayerController.Instance.TargetCharacter);

        var character = DialogueController.Instance.GetOrCreateDialogueCharacterData(Id);
        if (character == null)
        {
            Debug.LogError($"Character was null");
            Debug.Indent--;
            return;
        }

        var id = string.IsNullOrEmpty(character.StartNode) ? DefaultNode : character.StartNode;
        DialogueController.Instance.StartDialogue(new DialogueStartedArguments
        {
            Node = DialogueController.Instance.GetNode(id),
            Interactable = this,
        });

        DialogueController.Instance.OnDialogueEndedTemp += _ =>
        {
            EndInteraction();
        };

        Debug.Indent--;
    }

    protected override void EndInteraction()
    {
        base.EndInteraction();
        Character.Movement.StopLookingAt();
    }
}
