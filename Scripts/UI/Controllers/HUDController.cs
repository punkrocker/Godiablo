using Godot;
using Diablo.Core.Enums;
using Diablo.Core.Events;

namespace Diablo.Scripts.UI.Controllers;

/// <summary>
/// HUD控制器
/// 绑定到玩家属性变化信号，更新HP/MP/耐力条和交互提示
/// </summary>
public partial class HUDController : Control
{
    [Export] public ProgressBar HealthBar { get; set; }
    [Export] public ProgressBar ManaBar { get; set; }
    [Export] public ProgressBar StaminaBar { get; set; }
    [Export] public Label HealthLabel { get; set; }
    [Export] public Label ManaLabel { get; set; }
    [Export] public Label StaminaLabel { get; set; }
    [Export] public Label LevelLabel { get; set; }
    [Export] public Label InteractionPromptLabel { get; set; }
    [Export] public Panel CrosshairPanel { get; set; }

    public override void _Ready()
    {
        // auto-find nodes if not assigned in inspector
        if (HealthBar == null) HealthBar = GetNodeOrNull<ProgressBar>("StatsPanel/HealthBar");
        if (ManaBar == null) ManaBar = GetNodeOrNull<ProgressBar>("StatsPanel/ManaBar");
        if (StaminaBar == null) StaminaBar = GetNodeOrNull<ProgressBar>("StatsPanel/StaminaBar");
        if (HealthLabel == null) HealthLabel = GetNodeOrNull<Label>("StatsPanel/LabelsRow/HealthLabel");
        if (ManaLabel == null) ManaLabel = GetNodeOrNull<Label>("StatsPanel/LabelsRow/ManaLabel");
        if (StaminaLabel == null) StaminaLabel = GetNodeOrNull<Label>("StatsPanel/LabelsRow/StaminaLabel");
        if (LevelLabel == null) LevelLabel = GetNodeOrNull<Label>("StatsPanel/LabelsRow/LevelLabel");
        if (InteractionPromptLabel == null) InteractionPromptLabel = GetNodeOrNull<Label>("InteractionPromptLabel");
        if (CrosshairPanel == null) CrosshairPanel = GetNodeOrNull<Panel>("CrosshairPanel");

        GameEvents.OnPlayerStatsChanged += OnStatsChanged;
        GameEvents.OnPlayerLevelUp += OnLevelUp;
        GameEvents.OnInteractionAvailable += OnInteractionAvailable;
        GameEvents.OnInteractionCleared += OnInteractionCleared;

        if (InteractionPromptLabel != null)
            InteractionPromptLabel.Visible = false;
    }

    public override void _ExitTree()
    {
        GameEvents.OnPlayerStatsChanged -= OnStatsChanged;
        GameEvents.OnPlayerLevelUp -= OnLevelUp;
        GameEvents.OnInteractionAvailable -= OnInteractionAvailable;
        GameEvents.OnInteractionCleared -= OnInteractionCleared;
    }

    private void OnStatsChanged(StatType stat, float current, float max)
    {
        GD.Print($"[HUDController] OnStatsChanged: {stat} {current}/{max}");
        switch (stat)
        {
            case StatType.Health:
                UpdateBar(HealthBar, HealthLabel, current, max, "HP");
                break;
            case StatType.Mana:
                UpdateBar(ManaBar, ManaLabel, current, max, "MP");
                break;
            case StatType.Stamina:
                UpdateBar(StaminaBar, StaminaLabel, current, max, "SP");
                break;
        }
    }

    private void UpdateBar(ProgressBar bar, Label label, float current, float max, string prefix)
    {
        if (bar != null)
        {
            bar.MaxValue = max;
            bar.Value = current;
        }

        if (label != null)
        {
            label.Text = $"{prefix}: {current:F0}/{max:F0}";
        }
    }

    private void OnLevelUp(int newLevel)
    {
        if (LevelLabel != null)
        {
            LevelLabel.Text = $"Lv.{newLevel}";
        }
    }

    private void OnInteractionAvailable(string id, string prompt)
    {
        if (InteractionPromptLabel != null)
        {
            InteractionPromptLabel.Text = prompt;
            InteractionPromptLabel.Visible = true;
        }
    }

    private void OnInteractionCleared()
    {
        if (InteractionPromptLabel != null)
        {
            InteractionPromptLabel.Visible = false;
        }
    }
}
