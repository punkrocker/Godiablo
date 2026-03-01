using Diablo.Core.Enums;

namespace Diablo.UI.Data;

/// <summary>
/// UI任务条目数据
/// </summary>
public class UIQuestEntryData
{
    public string QuestId { get; set; } = "";
    public string Title { get; set; } = "";
    public string ShortDescription { get; set; } = "";
    public QuestState State { get; set; } = QuestState.NotStarted;
    public string[] ObjectiveSummaries { get; set; } = System.Array.Empty<string>();
    public bool IsMainQuest { get; set; } = false;
    public bool IsTracked { get; set; } = true;
}

