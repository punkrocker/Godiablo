namespace Diablo.NPC.Dialogue;

/// <summary>
/// 对话选项
/// 代表对话中的一个可选回复
/// </summary>
public class DialogueChoice
{
    /// <summary>选项显示文本</summary>
    public string Text { get; set; } = "";

    /// <summary>选择后跳转的目标节点ID</summary>
    public string TargetNodeId { get; set; } = "";

    /// <summary>显示条件（为空则始终显示）</summary>
    public string ConditionKey { get; set; } = "";

    /// <summary>选择后触发的任务ID</summary>
    public string TriggerQuestId { get; set; } = "";

    /// <summary>选择后触发的条件标志</summary>
    public string SetConditionKey { get; set; } = "";
}

