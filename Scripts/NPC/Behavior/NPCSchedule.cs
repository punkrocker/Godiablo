using Godot;
using System.Collections.Generic;

namespace Diablo.NPC.Behavior;

/// <summary>
/// NPC日程资源
/// 定义NPC在不同时间段的行为和位置
/// </summary>
[GlobalClass]
public partial class NPCSchedule : Resource
{
    /// <summary>
    /// 日程条目列表
    /// </summary>
    public List<ScheduleEntry> Entries { get; set; } = new();

    /// <summary>
    /// 根据当前时间获取对应的日程条目
    /// </summary>
    public ScheduleEntry GetCurrentEntry(float timeOfDay)
    {
        ScheduleEntry best = null;
        foreach (var entry in Entries)
        {
            if (timeOfDay >= entry.StartTime && (best == null || entry.StartTime > best.StartTime))
            {
                best = entry;
            }
        }
        return best ?? (Entries.Count > 0 ? Entries[0] : null);
    }
}

/// <summary>
/// 日程条目
/// </summary>
public class ScheduleEntry
{
    /// <summary>开始时间（0~24 小时制）</summary>
    public float StartTime { get; set; }

    /// <summary>目标位置</summary>
    public Vector3 TargetPosition { get; set; }

    /// <summary>到达后的行为</summary>
    public string Activity { get; set; } = "idle"; // idle, work, sleep, patrol

    /// <summary>到达后播放的动画</summary>
    public string AnimationName { get; set; } = "idle";
}

