using Godot;
using System.Collections.Generic;
using System.Text.Json;

public partial class DialogueController : Node
{
    public static DialogueController Instance => Singleton.TryGet<DialogueController>(out var instance) ? instance : Create();
    public static DialogueController Create() => Singleton.Create<DialogueController>($"Dialogue/{nameof(DialogueController)}");

    public DialogueView DialogueView => View.LoadSingleton<DialogueView>();

    private DialogueNodeCollection _collection;

    private Dictionary<string, string> _overwrites = new();

    public string SelectedUrlId { get; set; } = string.Empty;

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

    public DialogueCharacterData GetOrCreateDialogueCharacterData(string id)
    {
        Debug.Log($"DialogueView.GetOrCreateDialogueCharacterdata: {id}");
        Debug.Indent++;

        if (string.IsNullOrEmpty(id))
        {
            Debug.Log("id was null or empty");
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
            Debug.Log($"Set dialogue overwrite {key} to {overwrite}");
        }
        else
        {
            _overwrites.Add(key, overwrite);
            Debug.Log($"Added dialogue overwrite {key} to {overwrite}");
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
