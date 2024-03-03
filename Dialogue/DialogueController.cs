using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;

public partial class DialogueController : Node
{
    public static DialogueController Instance => Singleton.TryGet<DialogueController>(out var instance) ? instance : Create();
    public static DialogueController Create() => Singleton.Create<DialogueController>($"Dialogue/{nameof(DialogueController)}");

    public DialogueView DialogueView => View.LoadSingleton<DialogueView>();

    private DialogueNodeCollection _collection;

    private Dictionary<string, string> _overwrites = new();

    private DialogueNode CurrentNode;

    public bool IsDialogueActive { get; private set; }

    public Action<DialogueStartedArguments> OnDialogueStarted;
    public Action<DialogueEndedArguments> OnDialogueEnded;
    public Action<DialogueEndedArguments> OnDialogueEndedTemp;

    public override void _Ready()
    {
        base._Ready();
        DeserializeDialogue();
    }

    public DialogueNode GetNode(string id) => _collection.GetNode(id);

    private void DeserializeDialogue()
    {
        var path = Paths.DialogueJson;
        var content = FileAccess.GetFileAsString(path);
        var nodes = JsonSerializer.Deserialize<IEnumerable<DialogueNode>>(content);
        UpdateNodes(nodes);
        _collection = new DialogueNodeCollection(nodes);
    }

    private void UpdateNodes(IEnumerable<DialogueNode> nodes)
    {
        foreach (var node in nodes)
        {
            node.Text = node.Text
                .Replace("[url", "[color=yellow][url")
                .Replace("[/url]", "[/url][/color]");
        }
    }

    public void SetDialogueNode(string id) => SetDialogueNode(GetNode(id));
    public void NextDialogueText() => SetDialogueNode(CurrentNode.Next);

    private void SetDialogueNode(DialogueNode node)
    {
        Debug.TraceMethod(node?.Id);
        Debug.Indent++;

        if (CurrentNode == null && node != null)
        {
            DialogueView.ShowDialogueBox();
        }

        var previous_node = CurrentNode;
        CurrentNode = node;

        if (CurrentNode == null)
        {
            Debug.Indent--;

            EndDialogue(new DialogueEndedArguments
            {
                Node = previous_node
            });

            return;
        }

        Debug.Trace($"Dialogue node: {CurrentNode.Id}");

        ParseDialogueNode(node);
        DialogueView.SetDialogueNode(node);

        Debug.Indent--;
    }

    private void ParseDialogueNode(DialogueNode node)
    {
        Debug.TraceMethod(node?.Id);
        Debug.Indent++;

        if (node == null)
        {
            Debug.LogError("Node was null");
            Debug.Indent--;
            return;
        }

        ParseCharacter(node);

        Debug.Indent--;
    }

    private void ParseCharacter(DialogueNode node)
    {
        var id_character = string.IsNullOrEmpty(node.Character) ? "DEFAULT" : node.Character;
        var character = GetOrCreateDialogueCharacterData(id_character);

        if (!string.IsNullOrEmpty(node.Start))
        {
            Debug.Trace($"node.Start: {node.Start}");
            if (character == null)
            {
                Debug.LogError("Character was not found");
            }
            else
            {
                character.StartNode = node.Start;
            }
        }
    }

    public void StartDialogue(DialogueStartedArguments args)
    {
        Debug.TraceMethod(args?.Node?.Id);
        Debug.Indent++;

        IsDialogueActive = true;
        PlayerInput.Instance.InputLock.AddLock(nameof(DialogueController));

        OnDialogueStarted?.Invoke(args);
        SetDialogueNode(args.Node);

        Debug.Indent--;
    }

    private void EndDialogue(DialogueEndedArguments args)
    {
        CurrentNode = null;
        DialogueView.HideDialogueBox();

        IsDialogueActive = false;
        PlayerInput.Instance.InputLock.RemoveLock(nameof(DialogueController));

        if (args != null)
        {
            Debug.TraceMethod(args.Node?.Id);
            Debug.Indent++;

            OnDialogueEnded?.Invoke(args);
            OnDialogueEndedTemp?.Invoke(args);
            OnDialogueEndedTemp = null;

            Debug.Indent--;
        }
    }

    public DialogueCharacterData GetOrCreateDialogueCharacterData(string id)
    {
        Debug.TraceMethod(id);
        Debug.Indent++;

        if (string.IsNullOrEmpty(id))
        {
            Debug.Trace("id was null or empty");
            Debug.Indent--;
            return null;
        }

        if (!Save.Game.DialogueCharacters.TryGetValue(id, out var data))
        {
            data = new DialogueCharacterData
            {
                Id = id
            };

            Save.Game.DialogueCharacters.Add(id, data);
        }

        Debug.Indent--;
        return data;
    }

    public void SetOverwrite(string key, string overwrite)
    {
        if (_overwrites.ContainsKey(key))
        {
            _overwrites[key] = overwrite;
            Debug.Trace($"Set dialogue overwrite {key} to {overwrite}");
        }
        else
        {
            _overwrites.Add(key, overwrite);
            Debug.Trace($"Added dialogue overwrite {key} to {overwrite}");
        }
    }

    public string ReplaceOverwrites(string text)
    {
        string s = text;

        foreach (var kvp in _overwrites)
        {
            var key = $"{Constants.DIALOGUE_OVERWRITE_CHAR}{kvp.Key}";
            s = s.Replace(key, kvp.Value);
        }

        return s;
    }
}

public class DialogueStartedArguments
{
    public DialogueNode Node { get; set; }
    public Interactable Interactable { get; set; }
}

public class DialogueEndedArguments
{
    public DialogueNode Node { get; set; }
}