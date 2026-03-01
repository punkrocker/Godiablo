using Godot;
using System.Collections.Generic;

namespace Diablo.NPC.Dialogue;

/// <summary>
/// 对话数据资源 - 存储完整对话树
/// </summary>
[GlobalClass]
public partial class DialogueData : Resource
{
    [Export] public string DialogueId { get; set; } = "";
    [Export] public string SpeakerName { get; set; } = "";
    [Export] public string StartNodeId { get; set; } = "start";

    /// <summary>
    /// 对话节点列表（使用 NodeId 作为索引）
    /// </summary>
    public List<DialogueNode> Nodes { get; set; } = new();

    /// <summary>
    /// 根据ID获取对话节点
    /// </summary>
    public DialogueNode GetNode(string nodeId)
    {
        return Nodes.Find(n => n.NodeId == nodeId);
    }
}

