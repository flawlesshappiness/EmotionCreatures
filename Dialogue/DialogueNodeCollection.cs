using System.Collections.Generic;

public class DialogueNodeCollection
{
    private Dictionary<string, DialogueNode> _nodes = new();

    public DialogueNodeCollection()
    {
    }

    public DialogueNodeCollection(IEnumerable<DialogueNode> nodes)
    {
        foreach (var node in nodes)
        {
            AddNode(node);
        }
    }

    public void AddNode(DialogueNode node)
    {
        if (_nodes.ContainsKey(node.Id)) return;
        _nodes.Add(node.Id, node);
    }

    public DialogueNode GetNode(string id)
    {
        if (id == null) return null;

        if (!_nodes.ContainsKey(id))
        {
            Debug.Log($"DialogueNodeCollection.GetNode: Failed to get node with id: {id}");
            return null;
        }

        return _nodes[id];
    }
}
