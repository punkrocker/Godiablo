using System.Collections.Generic;

namespace Diablo.Quest;

/// <summary>
/// 任务奖励
/// </summary>
public class QuestReward
{
    /// <summary>经验值奖励</summary>
    public int XP { get; set; } = 0;

    /// <summary>金币奖励</summary>
    public int Gold { get; set; } = 0;

    /// <summary>物品奖励 (ItemId 列表)</summary>
    public List<string> ItemIds { get; set; } = new();

    /// <summary>解锁的条件标志</summary>
    public List<string> UnlockFlags { get; set; } = new();
}

