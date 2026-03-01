using System.Collections.Generic;

namespace Diablo.NPC.Dialogue;

/// <summary>
/// 单个对话节点
/// 包含说话者文本、分支选项和触发条件
/// </summary>
public class DialogueNode
{
    /// <summary>节点唯一ID</summary>
    public string NodeId { get; set; } = "";

    /// <summary>说话者名称（可覆盖 DialogueData 中的默认值）</summary>
    public string Speaker { get; set; } = "";

    /// <summary>对话文本</summary>
    public string Text { get; set; } = "";

    /// <summary>下一个节点ID（无选项时自动跳转）</summary>
    public string NextNodeId { get; set; } = "";

    /// <summary>对话选项列表</summary>
    public List<DialogueChoice> Choices { get; set; } = new();

    /// <summary>显示此节点所需的条件键</summary>
    public string ConditionKey { get; set; } = "";

    /// <summary>是否为对话结束节点</summary>
    public bool IsEndNode { get; set; } = false;

    /// <summary>触发的事件（如接受任务）</summary>
    public string TriggerEvent { get; set; } = "";

    /// <summary>触发事件的参数</summary>
    public string TriggerEventParam { get; set; } = "";
}

