using Godot;
using System.Collections.Generic;
using Diablo.Core.Enums;
using Diablo.Core.Events;

namespace Diablo.Quest;

/// <summary>
/// 任务日志
/// 管理玩家所有接受的、已完成的和失败的任务
/// </summary>
public partial class QuestJournal : Node
{
    public List<QuestInstance> ActiveQuests { get; private set; } = new();
    public List<QuestInstance> CompletedQuests { get; private set; } = new();
    public List<QuestInstance> FailedQuests { get; private set; } = new();

    [Signal] public delegate void QuestAddedEventHandler(string questId);
    [Signal] public delegate void QuestCompletedEventHandler(string questId);
    [Signal] public delegate void QuestFailedEventHandler(string questId);
    [Signal] public delegate void ObjectiveProgressedEventHandler(string questId, int objectiveIndex, int current, int required);

    /// <summary>
    /// 接受新任务
    /// </summary>
    public bool AcceptQuest(QuestData questData)
    {
        if (questData == null) return false;

        // 检查是否已接受
        if (ActiveQuests.Exists(q => q.Data.QuestId == questData.QuestId)) return false;

        // 检查是否已完成且不可重复
        if (!questData.IsRepeatable && CompletedQuests.Exists(q => q.Data.QuestId == questData.QuestId))
            return false;

        var instance = new QuestInstance(questData);
        ActiveQuests.Add(instance);

        EmitSignal(SignalName.QuestAdded, questData.QuestId);
        GameEvents.EmitQuestAccepted(questData.QuestId);

        return true;
    }

    /// <summary>
    /// 推进任务目标
    /// </summary>
    public void AdvanceObjective(QuestObjectiveType type, string targetId, int amount = 1)
    {
        for (int q = ActiveQuests.Count - 1; q >= 0; q--)
        {
            var quest = ActiveQuests[q];
            if (quest.AdvanceObjectiveByTarget(type, targetId, amount))
            {
                // 查找被推进的目标索引
                for (int i = 0; i < quest.LiveObjectives.Count; i++)
                {
                    var obj = quest.LiveObjectives[i];
                    if (obj.Type == type && obj.TargetId == targetId)
                    {
                        EmitSignal(SignalName.ObjectiveProgressed, quest.Data.QuestId,
                            i, obj.CurrentCount, obj.RequiredCount);
                        GameEvents.EmitQuestObjectiveUpdated(quest.Data.QuestId, i, obj.CurrentCount);
                    }
                }

                // 检查是否全部完成
                if (quest.AreAllObjectivesCompleted())
                {
                    CompleteQuest(quest);
                }
            }
        }
    }

    /// <summary>
    /// 完成任务
    /// </summary>
    private void CompleteQuest(QuestInstance quest)
    {
        quest.State = QuestState.Completed;
        ActiveQuests.Remove(quest);
        CompletedQuests.Add(quest);

        EmitSignal(SignalName.QuestCompleted, quest.Data.QuestId);
        GameEvents.EmitQuestCompleted(quest.Data.QuestId);
    }

    /// <summary>
    /// 使任务失败
    /// </summary>
    public void FailQuest(string questId)
    {
        var quest = ActiveQuests.Find(q => q.Data.QuestId == questId);
        if (quest == null) return;

        quest.State = QuestState.Failed;
        ActiveQuests.Remove(quest);
        FailedQuests.Add(quest);

        EmitSignal(SignalName.QuestFailed, questId);
        GameEvents.EmitQuestFailed(questId);
    }

    /// <summary>
    /// 检查任务是否已完成
    /// </summary>
    public bool IsQuestCompleted(string questId)
    {
        return CompletedQuests.Exists(q => q.Data.QuestId == questId);
    }

    /// <summary>
    /// 获取当前活跃任务
    /// </summary>
    public QuestInstance GetActiveQuest(string questId)
    {
        return ActiveQuests.Find(q => q.Data.QuestId == questId);
    }
}

