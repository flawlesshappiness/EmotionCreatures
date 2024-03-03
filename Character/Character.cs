using Godot;
using System;

public partial class Character : CharacterBody3D
{
    public Action OnBeginTarget, OnEndTarget;

    [NodeType(typeof(CharacterMovement))]
    public CharacterMovement Movement;

    [NodeType(typeof(CharacterAnimator))]
    public CharacterAnimator Animator;

    [NodeType(typeof(CharacterNavigation))]
    public CharacterNavigation Navigation;

    [NodeName(nameof(ShoulderVCam))]
    public VirtualCamera ShoulderVCam;

    [NodeName(nameof(FaceNode))]
    public Node3D FaceNode;

    public VirtualCamera ThirdPersonVCam { get; private set; }

    public AI AI { get; private set; }

    public bool IsPlayer => PlayerController.Instance.TargetCharacter == this;
    public bool CanControl => !(AI != null && AI.Active);

    public override void _Ready()
    {
        base._Ready();

        NodeScript.FindNodesFromAttribute(this, GetType());

        Movement.Initialize(this);
        Animator.Initialize(this);
        Navigation.Initialize(this);
        InitializeCamera();

        DialogueController.Instance.OnDialogueStarted += OnDialogueStarted;
        DialogueController.Instance.OnDialogueEnded += OnDialogueEnded;
        PlayerInput.Instance.InputLock.OnLocked += OnInputLocked;
    }

    private void InitializeCamera()
    {
        ThirdPersonVCam = CameraController.Instance.CreateThirdPersonVirtualCamera();
        ThirdPersonVCam.FollowTarget = FaceNode;
        ThirdPersonVCam.LookTarget = FaceNode;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (AI != null)
        {
            AI.Process();
        }
    }

    public virtual void BeginTarget()
    {
        OnBeginTarget?.Invoke();
    }

    public virtual void EndTarget()
    {
        OnEndTarget?.Invoke();
    }

    public void SetAI(AI ai)
    {
        AI = ai;
        AI.Initialize(this);
    }

    private void OnDialogueStarted(DialogueStartedArguments args)
    {
        if (!IsPlayer) return;

        Movement.StartLookingAt(args.Interactable);

        var interact_character = args.Interactable.GetNodeInParents<Character>();
        if (interact_character != null)
        {
            ShoulderVCam.LookTarget = interact_character.FaceNode;
        }

        ShoulderVCam.MoveTo(0.25f, Curves.EaseOutQuad);
    }

    private void OnDialogueEnded(DialogueEndedArguments args)
    {
        if (!IsPlayer) return;

        Movement.StopLookingAt();
        ThirdPersonVCam.MoveTo(0.25f, Curves.EaseOutQuad);
    }

    private void OnInputLocked()
    {
        if (!IsPlayer) return;

        Movement.Stop();
    }
}
