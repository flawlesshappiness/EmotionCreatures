using Godot;
using System;
using System.Collections.Generic;

public class DialogueText
{
    public DialogueNode Node { get; private set; }

    public string Text { get; private set; }

    public int TextLength { get; private set; }

    public List<IndexAnimation> Animations { get; private set; } = new();

    public abstract class IndexAnimation
    {
        public int Index { get; set; }
    }

    public class PauseIndexAnimation : IndexAnimation
    {
        public int DurationInMs { get; set; }
    }

    public DialogueText(DialogueNode node)
    {
        Node = node;
        ParseText(Node.Text);
        OverwriteText();
        Debug.Log("Text: " + Text);
    }

    private void OverwriteText()
    {
        if (!Node.Text.Contains(Constants.DIALOGUE_OVERWRITE_CHAR))
        {
            return;
        }

        Text = DialogueController.Instance.ReplaceOverwrites(Text);
    }

    private void ParseText(string str)
    {
        Text = "";
        TextLength = 0;

        Debug.Log($"Parsing text: {str}");
        Debug.Indent++;

        for (int i = 0; i < str.Length; i++)
        {
            // BBCode
            if (str[i] == '[')
            {
                CaptureUntil(str, i, ']', out var captured);
                TextLength -= captured.Length;
                Debug.Log("BBCode: " + captured);
            }

            // Index Animation
            if (str[i] == '{')
            {
                i += CaptureUntil(str, i, '}', out var captured);
                ParseIndexAnimation(TextLength, captured);
            }

            Text += str[i];
            TextLength++;
        }

        Debug.Indent--;
    }

    private int CaptureUntil(string str, int i, char c, out string captured_string)
    {
        var start = i;
        captured_string = "";
        i++;

        while (i < str.Length && str[i] != c)
        {
            captured_string += str[i];
            i++;
        }

        i++;
        return i - start;
    }

    private void ParseIndexAnimation(int index, string captured_string)
    {
        _ = captured_string ?? throw new ArgumentNullException(nameof(captured_string));

        Debug.Log($"Index Animation ({index}): {captured_string}");
        Debug.Indent++;

        try
        {
            var parts = captured_string.Split('=');
            switch (parts[0])
            {
                case "pause":
                    ParsePauseIndexAnimation(index, parts[1]);
                    break;

                default:
                    throw new ArgumentException(parts[0]);
            }
        }
        catch (Exception e)
        {
            GD.PrintErr($"Failed to parse IndexAnimation from captured string: {captured_string}");
            GD.PrintErr($"{e.GetType().Name}: {e.Message}");
            GD.Print(e.StackTrace);
        }

        Debug.Indent--;
    }

    private void ParsePauseIndexAnimation(int index, string value)
    {
        _ = value ?? throw new ArgumentNullException(nameof(value));

        if (int.TryParse(value, out var duration))
        {
            var animation = new PauseIndexAnimation
            {
                Index = index,
                DurationInMs = duration
            };

            Animations.Add(animation);
        }
        else
        {
            throw new ArgumentException(nameof(value));
        }
    }
}
