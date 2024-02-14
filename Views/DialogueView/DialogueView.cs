using Godot;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class DialogueView : View
{
    [NodeName(nameof(DialogueArrow))]
    public Control DialogueArrow;

    [NodeName(nameof(DialogueLabel))]
    public RichTextLabel DialogueLabel;

    [NodeName(nameof(DialogueOptions))]
    public DialogueOptionContainer DialogueOptions;

    private Coroutine _cr_dialogue_text;

    private const ulong MSEC_PER_CHAR = 15;

    private bool _first_frame;
    private double time_dialogue_sfx_play;
    private double duration_dialogue_sfx;

    public bool IsAnimatingDialogue => !(_cr_dialogue_text?.HasEnded ?? false);

    public override void _Ready()
    {
        base._Ready();
        DialogueOptions.Hide();
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        InputMouseButton(@event as InputEventMouseButton);
        PressSubmit();
        PressUp();
        PressDown();
    }

    private void InputMouseButton(InputEventMouseButton e)
    {
        if (e == null) return;
        if (!Visible) return;
        if (DialogueOptions.HasOptions) return;

        if (e.IsReleased() && e.ButtonIndex == MouseButton.Left)
        {
            if (IsAnimatingDialogue)
            {
                EndAnimateDialogueText();
            }
        }
    }

    private void PressSubmit()
    {
        if (!Visible) return;

        if (Input.IsActionJustPressed(PlayerControls.Submit))
        {
            if (IsAnimatingDialogue)
            {
                EndAnimateDialogueText();
            }
            else if (DialogueOptions.HasOptions)
            {
                DialogueOptions.Submit();
            }
            else
            {
                DialogueController.Instance.NextDialogueText();
            }
        }
    }

    private void PressUp()
    {
        if (Input.IsActionJustPressed(PlayerControls.Forward))
        {
            DialogueOptions.PreviousOption();
        }
    }

    private void PressDown()
    {
        if (Input.IsActionJustPressed(PlayerControls.Back))
        {
            DialogueOptions.NextOption();
        }
    }

    public void ShowDialogueBox()
    {
        PlayerInput.Instance.MouseVisibleLock.AddLock(nameof(DialogueView));
        Show();
    }

    public void HideDialogueBox()
    {
        PlayerInput.Instance.MouseVisibleLock.RemoveLock(nameof(DialogueView));
        Hide();
    }

    public void SetDialogueNode(DialogueNode node)
    {
        var text = new DialogueText(node);
        DialogueArrow.Hide();
        DialogueOptions.Hide();
        SetDialogueText(text);
        AnimateDialogueText(text);
        SetDialogueOptions(node.Options);
    }

    private void SetDialogueOptions(List<DialogueNodeOption> options)
    {
        if (options.Count == 0)
        {
            DialogueOptions.Clear();
        }
        else
        {
            DialogueOptions.CreateOptions(options);
        }
    }

    public void SetDialogueText(DialogueText text)
    {
        DialogueLabel.Text = text.Text;
        DialogueLabel.VisibleCharacters = -1;
    }

    public Coroutine AnimateDialogueText(DialogueText text) => AnimateDialogueText(text, MSEC_PER_CHAR);

    private Coroutine AnimateDialogueText(DialogueText text, ulong msec_per_char)
    {
        DialogueLabel.VisibleCharacters = 0;
        _first_frame = true;

        Coroutine.Stop(_cr_dialogue_text);
        _cr_dialogue_text = Coroutine.Start(AnimateDialogueTextCr(text, msec_per_char));
        return _cr_dialogue_text;
    }

    private IEnumerator AnimateDialogueTextCr(DialogueText text, ulong msec_per_char)
    {
        var i = 0;
        var max = text.TextLength;
        var time_current = Time.GetTicksMsec();

        while (i < max)
        {
            yield return null;

            var previous_visible_characters = 0;
            while (i < max && time_current < Time.GetTicksMsec())
            {
                i++;
                DialogueLabel.VisibleCharacters = i;
                time_current += msec_per_char;

                if (i > previous_visible_characters)
                {
                    previous_visible_characters = i;
                }

                // Animation
                var animation = text.Animations.FirstOrDefault(a => a.Index == i);
                if (animation != null)
                {
                    var pause = animation as DialogueText.PauseIndexAnimation;
                    if (pause != null)
                    {
                        time_current += (ulong)pause.DurationInMs;
                        yield return new WaitForSeconds((float)pause.DurationInMs / 1000);
                    }
                }
            }

            _first_frame = false;
        }

        OnAnimateDialogueTextEnd();
    }

    private void OnAnimateDialogueTextEnd()
    {
        DialogueLabel.VisibleCharacters = -1;

        if (DialogueOptions.HasOptions)
        {
            DialogueOptions.Show();
        }
        else
        {
            DialogueArrow.Show();
        }
    }

    private void EndAnimateDialogueText()
    {
        if (_first_frame) return;
        Coroutine.Stop(_cr_dialogue_text);
        OnAnimateDialogueTextEnd();
    }
}

public class DialogueEndedArguments
{
    public DialogueNode Node { get; set; }
}