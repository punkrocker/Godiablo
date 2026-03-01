using System.Collections.Generic;
using Diablo.Core.Enums;

namespace Diablo.Quest;

/// <summary>
/// 任务运行时实例
/// 包装 QuestData，维护实时进度状态
/// </summary>
public class QuestInstance
{
    /// <summary>原始任务数据</summary>
    public QuestData Data { get; private set; }

    /// <summary>当前任务状态</summary>
    public QuestState State { get; set; } = QuestState.NotStarted;

    /// <summary>运行时任务目标（包含实时进度）</summary>
    public List<QuestObjective> LiveObjectives { get; private set; } = new();

    public QuestInstance(QuestData data)
    {
        Data = data;
        State = QuestState.InProgress;

        // 克隆目标以追踪进度
        foreach (var obj in data.Objectives)
        {
            LiveObjectives.Add(obj.Clone());
        }
    }

    /// <summary>
    /// 检查所有目标是否完成
    /// </summary>
    public bool AreAllObjectivesCompleted()
    {
        foreach (var obj in LiveObjectives)
        {
            if (!obj.IsCompleted) return false;
        }
        return true;
    }

    /// <summary>
    /// 推进指定目标的进度
    /// </summary>
    public bool AdvanceObjective(int objectiveIndex, int amount = 1)
    {
        if (objectiveIndex < 0 || objectiveIndex >= LiveObjectives.Count) return false;
        return LiveObjectives[objectiveIndex].Advance(amount);
    }

    /// <summary>
    /// 根据目标类型和目标ID推进进度
    /// </summary>
    public bool AdvanceObjectiveByTarget(QuestObjectiveType type, string targetId, int amount = 1)
    {
        bool advanced = false;
        for (int i = 0; i < LiveObjectives.Count; i++)
        {
            var obj = LiveObjectives[i];
            if (obj.Type == type && obj.TargetId == targetId && !obj.IsCompleted)
            {
                obj.Advance(amount);
                advanced = true;
            }
        }
        return advanced;
    }
}

