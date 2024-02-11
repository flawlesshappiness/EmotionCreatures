using Godot;
using System.Collections;

public partial class BattleAnimationView : View
{
    [NodeName(nameof(Background))]
    public ColorRect Background;

    [NodeName(nameof(Label))]
    public Label Label;

    public override void _Ready()
    {
        base._Ready();

        Background.Color = Colors.Transparent;
        Label.Visible = false;
    }

    public Coroutine AnimateDefaultBattleOpening()
    {
        return Coroutine.Start(Cr);
        IEnumerator Cr()
        {
            yield return AnimateBackgroundFade(true, 0f);
            Label.Visible = true;
            yield return new WaitForSeconds(1f);
        }
    }

    public Coroutine AnimateBackgroundFade(bool show, float duration)
    {
        var hide_color = new Color(0, 0, 0, 0);
        var show_color = Colors.Black;
        return Coroutine.Start(Cr);
        IEnumerator Cr()
        {
            var start = show ? hide_color : show_color;
            var end = show ? show_color : hide_color;
            yield return LerpEnumerator.Lerp01(duration, f =>
            {
                Background.Color = start.Lerp(end, f);
            });
        }
    }
}
