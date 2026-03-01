using Diablo.Core.Enums;

namespace Diablo.Quest;

/// <summary>
/// 任务目标
/// 描述完成任务需要达成的具体条件
/// </summary>
public class QuestObjective
{
    /// <summary>目标类型</summary>
    public QuestObjectiveType Type { get; set; } = QuestObjectiveType.Kill;

    /// <summary>目标实体/物品ID</summary>
    public string TargetId { get; set; } = "";

    /// <summary>目标描述</summary>
    public string Description { get; set; } = "";

    /// <summary>需要完成的数量</summary>
    public int RequiredCount { get; set; } = 1;

    /// <summary>当前已完成的数量</summary>
    public int CurrentCount { get; set; } = 0;

    /// <summary>是否已完成</summary>
    public bool IsCompleted => CurrentCount >= RequiredCount;

    /// <summary>
    /// 推进进度
    /// </summary>
    public bool Advance(int amount = 1)
    {
        if (IsCompleted) return false;
        CurrentCount += amount;
        if (CurrentCount > RequiredCount) CurrentCount = RequiredCount;
        return true;
    }

    /// <summary>
    /// 创建运行时副本
    /// </summary>
    public QuestObjective Clone()
    {
        return new QuestObjective
        {
            Type = Type,
            TargetId = TargetId,
            Description = Description,
            RequiredCount = RequiredCount,
            CurrentCount = 0
        };
    }
}

