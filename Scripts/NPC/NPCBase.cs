using Godot;

namespace Diablo.NPC;

/// <summary>
/// NPC基类
/// 提供交互提示、阵营归属等基础功能
/// </summary>
public partial class NPCBase : CharacterBody3D
{
    [Export] public string NpcId { get; set; } = "";
    [Export] public string NpcName { get; set; } = "NPC";
    [Export] public string Faction { get; set; } = "Neutral";
    [Export] public string InteractionPrompt { get; set; } = "对话";
    [Export] public Dialogue.DialogueData DialogueResource { get; set; }

    protected AnimationPlayer AnimPlayer;
    protected bool IsInteracting = false;

    [Signal] public delegate void InteractionStartedEventHandler(string npcId);
    [Signal] public delegate void InteractionEndedEventHandler(string npcId);

    public override void _Ready()
    {
        AnimPlayer = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");

        if (string.IsNullOrEmpty(NpcId))
        {
            NpcId = Name;
        }

        // 添加到NPC组以便查找
        AddToGroup("NPC");
    }

    /// <summary>
    /// 被玩家交互时调用（通过 Call("OnInteract") 触发）
    /// </summary>
    public virtual void OnInteract(Node interactor)
    {
        if (IsInteracting) return;

        IsInteracting = true;
        EmitSignal(SignalName.InteractionStarted, NpcId);

        // 面向交互者
        if (interactor is Node3D node3D)
        {
            var lookTarget = node3D.GlobalPosition;
            lookTarget.Y = GlobalPosition.Y;
            LookAt(lookTarget);
        }

        // 开始对话
        if (DialogueResource != null)
        {
            Dialogue.DialogueManager.Instance?.StartDialogue(NpcId, DialogueResource);
        }

        AnimPlayer?.Play("talk");
    }

    /// <summary>
    /// 结束交互
    /// </summary>
    public virtual void EndInteraction()
    {
        IsInteracting = false;
        EmitSignal(SignalName.InteractionEnded, NpcId);
        AnimPlayer?.Play("idle");
    }
}

