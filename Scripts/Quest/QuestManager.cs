using Godot;
using Diablo.Core.Enums;
using Diablo.Core.Events;

namespace Diablo.Quest;

/// <summary>
/// 任务管理器（单例/Autoload）
/// 监听全局事件，自动推进任务进度
/// </summary>
public partial class QuestManager : Node
{
    public static QuestManager Instance { get; private set; }

    [Export] public QuestJournal Journal { get; set; }

    public override void _Ready()
    {
        Instance = this;

        if (Journal == null)
        {
            Journal = GetNodeOrNull<QuestJournal>("QuestJournal");
        }

        // 订阅全局事件
        GameEvents.OnEntityDeath += OnEntityKilled;
        GameEvents.OnItemPickedUp += OnItemPickedUp;
        GameEvents.OnQuestAccepted += OnQuestAccepted;
    }

    public override void _ExitTree()
    {
        GameEvents.OnEntityDeath -= OnEntityKilled;
        GameEvents.OnItemPickedUp -= OnItemPickedUp;
        GameEvents.OnQuestAccepted -= OnQuestAccepted;
    }

    /// <summary>
    /// 实体被击杀时推进击杀类任务
    /// </summary>
    private void OnEntityKilled(string entityId, string killerId)
    {
        Journal?.AdvanceObjective(QuestObjectiveType.Kill, entityId);
    }

    /// <summary>
    /// 拾取物品时推进收集类任务
    /// </summary>
    private void OnItemPickedUp(string itemId, int quantity)
    {
        Journal?.AdvanceObjective(QuestObjectiveType.Collect, itemId, quantity);
    }

    /// <summary>
    /// 处理任务接受事件（可用于加载任务数据）
    /// </summary>
    private void OnQuestAccepted(string questId)
    {
        GD.Print($"[任务] 接受任务: {questId}");
    }

    /// <summary>
    /// 手动推进"对话"类型目标
    /// </summary>
    public void AdvanceTalkObjective(string npcId)
    {
        Journal?.AdvanceObjective(QuestObjectiveType.TalkTo, npcId);
    }

    /// <summary>
    /// 手动推进"探索"类型目标
    /// </summary>
    public void AdvanceExploreObjective(string locationId)
    {
        Journal?.AdvanceObjective(QuestObjectiveType.Explore, locationId);
    }
}

