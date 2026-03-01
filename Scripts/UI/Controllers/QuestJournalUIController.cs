using Godot;
using System.Collections.Generic;
using Diablo.Quest;
using Diablo.UI.Data;

namespace Diablo.UI.Controllers;

/// <summary>
/// 任务日志UI控制器
/// 显示任务列表、目标进度和状态
/// </summary>
public partial class QuestJournalUIController : Control
{
    [Export] public QuestJournal Journal { get; set; }
    [Export] public VBoxContainer QuestListContainer { get; set; }
    [Export] public Label QuestTitleLabel { get; set; }
    [Export] public RichTextLabel QuestDescriptionLabel { get; set; }
    [Export] public VBoxContainer ObjectivesContainer { get; set; }
    [Export] public PackedScene QuestEntryScene { get; set; }

    private string _selectedQuestId = "";

    public override void _Ready()
    {
        Visible = false;

        if (Journal != null)
        {
            Journal.QuestAdded += OnQuestAdded;
            Journal.QuestCompleted += OnQuestCompleted;
            Journal.ObjectiveProgressed += OnObjectiveProgressed;
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("open_quest_journal"))
        {
            Visible = !Visible;
            if (Visible) RefreshQuestList();
        }
    }

    public void RefreshQuestList()
    {
        if (QuestListContainer == null || Journal == null) return;

        foreach (var child in QuestListContainer.GetChildren())
        {
            child.QueueFree();
        }

        foreach (var quest in Journal.ActiveQuests)
        {
            AddQuestEntry(quest);
        }
    }

    private void AddQuestEntry(QuestInstance quest)
    {
        Button button;
        if (QuestEntryScene != null)
        {
            button = QuestEntryScene.Instantiate<Button>();
        }
        else
        {
            button = new Button();
        }

        string prefix = quest.Data.IsMainQuest ? "[主线] " : "[支线] ";
        button.Text = prefix + quest.Data.Title;

        string questId = quest.Data.QuestId;
        button.Pressed += () => SelectQuest(questId);

        QuestListContainer?.AddChild(button);
    }

    private void SelectQuest(string questId)
    {
        _selectedQuestId = questId;
        var quest = Journal?.GetActiveQuest(questId);
        if (quest == null) return;

        if (QuestTitleLabel != null)
            QuestTitleLabel.Text = quest.Data.Title;

        if (QuestDescriptionLabel != null)
            QuestDescriptionLabel.Text = quest.Data.Description;

        // 显示目标
        if (ObjectivesContainer != null)
        {
            foreach (var child in ObjectivesContainer.GetChildren())
            {
                child.QueueFree();
            }

            foreach (var obj in quest.LiveObjectives)
            {
                var label = new Label();
                string status = obj.IsCompleted ? "✓" : "○";
                label.Text = $"  {status} {obj.Description} ({obj.CurrentCount}/{obj.RequiredCount})";
                ObjectivesContainer.AddChild(label);
            }
        }
    }

    private void OnQuestAdded(string questId) => RefreshQuestList();
    private void OnQuestCompleted(string questId) => RefreshQuestList();
    private void OnObjectiveProgressed(string questId, int idx, int current, int required)
    {
        if (questId == _selectedQuestId)
        {
            SelectQuest(questId);
        }
    }
}

