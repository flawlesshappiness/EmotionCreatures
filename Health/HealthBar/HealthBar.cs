using Godot;
using System.Collections;

public partial class HealthBar : Sprite3D
{
    [Export]
    public Texture2D TextureProgressRed;

    [Export]
    public Texture2D TextureProgressBlue;

    [NodeName(nameof(ProgressBarRed))]
    public TextureProgressBar ProgressBarRed;

    [NodeName(nameof(ProgressBarWhite))]
    public TextureProgressBar ProgressBarWhite;

    [NodeName(nameof(SubViewport))]
    public SubViewport SubViewport;

    private Health Health { get; set; }

    private Coroutine cr_update;

    public override void _Ready()
    {
        base._Ready();
        NodeScript.FindNodesFromAttribute(this, GetType());

        Texture = SubViewport.GetTexture();
    }

    public void SetBlue() => ProgressBarRed.TextureProgress = TextureProgressBlue;
    public void SetRed() => ProgressBarRed.TextureProgress = TextureProgressRed;

    public void SubscribeTo(Health health)
    {
        Health = health;
        Health.OnValueChanged += OnValueChanged;

        ProgressBarRed.Value = health.Percentage;
        ProgressBarWhite.Value = health.Percentage;
    }

    private void OnValueChanged()
    {
        AnimateUpdate();
    }

    private void AnimateUpdate()
    {
        if (cr_update != null)
        {
            Coroutine.Stop(cr_update);
            cr_update = null;
        }

        cr_update = Coroutine.Start(Cr);
        IEnumerator Cr()
        {
            var curve = Curves.Linear;
            var start = ProgressBarWhite.Value;
            var end = Health.Percentage;

            ProgressBarRed.Value = end;
            yield return new WaitForSeconds(0.5f);
            yield return LerpEnumerator.Lerp01(0.5f, f =>
            {
                var c = curve.Evaluate(f);
                var v = Mathf.Lerp(start, end, c);
                ProgressBarWhite.Value = v;
            });
        }
    }
}
