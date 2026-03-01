using Godot;
using System.Collections.Generic;
using Diablo.Core.Events;

namespace Diablo.NPC.Dialogue;

/// <summary>
/// 对话管理器（单例/Autoload）
/// 管理对话流程、推进对话、处理选项
/// </summary>
public partial class DialogueManager : Node
{
    public static DialogueManager Instance { get; private set; }

    private DialogueData _currentDialogue;
    private DialogueNode _currentNode;
    private string _currentNpcId;

    /// <summary>全局条件标志（用于对话分支判断）</summary>
    public HashSet<string> ConditionFlags { get; set; } = new();

    [Signal] public delegate void DialogueNodeDisplayedEventHandler(
        string speaker, string text, string[] choices);
    [Signal] public delegate void DialogueEndedEventHandler();

    public override void _Ready()
    {
        Instance = this;
    }

    /// <summary>
    /// 开始一段对话
    /// </summary>
    public void StartDialogue(string npcId, DialogueData dialogueData)
    {
        _currentDialogue = dialogueData;
        _currentNpcId = npcId;

        GameEvents.EmitDialogueStarted(npcId);
        DisplayNode(dialogueData.StartNodeId);
    }

    /// <summary>
    /// 显示指定对话节点
    /// </summary>
    public void DisplayNode(string nodeId)
    {
        if (_currentDialogue == null) return;

        _currentNode = _currentDialogue.GetNode(nodeId);
        if (_currentNode == null || _currentNode.IsEndNode)
        {
            EndDialogue();
            return;
        }

        // 检查条件
        if (!string.IsNullOrEmpty(_currentNode.ConditionKey) &&
            !ConditionFlags.Contains(_currentNode.ConditionKey))
        {
            // 条件不满足，跳到下一个节点
            if (!string.IsNullOrEmpty(_currentNode.NextNodeId))
            {
                DisplayNode(_currentNode.NextNodeId);
            }
            else
            {
                EndDialogue();
            }
            return;
        }

        // 处理触发事件
        if (!string.IsNullOrEmpty(_currentNode.TriggerEvent))
        {
            HandleTriggerEvent(_currentNode.TriggerEvent, _currentNode.TriggerEventParam);
        }

        // 获取可用选项
        var availableChoices = GetAvailableChoices();
        var choiceTexts = new string[availableChoices.Count];
        for (int i = 0; i < availableChoices.Count; i++)
        {
            choiceTexts[i] = availableChoices[i].Text;
        }

        string speaker = string.IsNullOrEmpty(_currentNode.Speaker)
            ? _currentDialogue.SpeakerName
            : _currentNode.Speaker;

        EmitSignal(SignalName.DialogueNodeDisplayed, speaker, _currentNode.Text, choiceTexts);
    }

    /// <summary>
    /// 选择对话选项
    /// </summary>
    public void SelectChoice(int choiceIndex)
    {
        var choices = GetAvailableChoices();
        if (choiceIndex < 0 || choiceIndex >= choices.Count) return;

        var choice = choices[choiceIndex];

        // 设置条件标志
        if (!string.IsNullOrEmpty(choice.SetConditionKey))
        {
            ConditionFlags.Add(choice.SetConditionKey);
        }

        // 触发任务
        if (!string.IsNullOrEmpty(choice.TriggerQuestId))
        {
            GameEvents.EmitQuestAccepted(choice.TriggerQuestId);
        }

        DisplayNode(choice.TargetNodeId);
    }

    /// <summary>
    /// 推进对话（无选项时点击继续）
    /// </summary>
    public void AdvanceDialogue()
    {
        if (_currentNode == null) return;

        if (_currentNode.Choices.Count == 0 && !string.IsNullOrEmpty(_currentNode.NextNodeId))
        {
            DisplayNode(_currentNode.NextNodeId);
        }
    }

    /// <summary>
    /// 结束对话
    /// </summary>
    public void EndDialogue()
    {
        GameEvents.EmitDialogueEnded(_currentNpcId);
        EmitSignal(SignalName.DialogueEnded);

        _currentDialogue = null;
        _currentNode = null;
        _currentNpcId = "";
    }

    private List<DialogueChoice> GetAvailableChoices()
    {
        if (_currentNode == null) return new List<DialogueChoice>();

        var available = new List<DialogueChoice>();
        foreach (var choice in _currentNode.Choices)
        {
            if (string.IsNullOrEmpty(choice.ConditionKey) ||
                ConditionFlags.Contains(choice.ConditionKey))
            {
                available.Add(choice);
            }
        }
        return available;
    }

    private void HandleTriggerEvent(string eventName, string param)
    {
        switch (eventName)
        {
            case "accept_quest":
                GameEvents.EmitQuestAccepted(param);
                break;
            case "set_flag":
                ConditionFlags.Add(param);
                break;
        }
    }
}

