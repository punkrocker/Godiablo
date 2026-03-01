using Godot;
using System.Collections.Generic;

namespace Diablo.Quest;

/// <summary>
/// 任务数据资源
/// 定义任务的基本信息、目标和奖励
/// </summary>
[GlobalClass]
public partial class QuestData : Resource
{
    [Export] public string QuestId { get; set; } = "";
    [Export] public string Title { get; set; } = "未命名任务";
    [Export(PropertyHint.MultilineText)] public string Description { get; set; } = "";
    [Export] public int RecommendedLevel { get; set; } = 1;

    /// <summary>前置任务ID列表</summary>
    public List<string> PrerequisiteQuestIds { get; set; } = new();

    /// <summary>任务目标列表</summary>
    public List<QuestObjective> Objectives { get; set; } = new();

    /// <summary>任务奖励</summary>
    public QuestReward Reward { get; set; } = new();

    /// <summary>是否为主线任务</summary>
    [Export] public bool IsMainQuest { get; set; } = false;

    /// <summary>是否可重复</summary>
    [Export] public bool IsRepeatable { get; set; } = false;
}

