using Godot;
using System.Collections.Generic;
using Diablo.Loot;

namespace Diablo.UI.Controllers;

/// <summary>
/// 战利品弹窗UI控制器
/// 显示从敌人/宝箱掉落的战利品
/// </summary>
public partial class LootPopupUIController : Control
{
    [Export] public VBoxContainer LootListContainer { get; set; }
    [Export] public Button TakeAllButton { get; set; }
    [Export] public Button CloseButton { get; set; }
    [Export] public Label TitleLabel { get; set; }

    private List<LootResult> _currentLoot = new();

    [Signal] public delegate void LootTakenEventHandler(int index);
    [Signal] public delegate void AllLootTakenEventHandler();

    public override void _Ready()
    {
        Visible = false;

        if (TakeAllButton != null)
            TakeAllButton.Pressed += OnTakeAll;

        if (CloseButton != null)
            CloseButton.Pressed += Close;
    }

    /// <summary>
    /// 显示战利品
    /// </summary>
    public void ShowLoot(List<LootResult> lootResults, string sourceName = "战利品")
    {
        _currentLoot = lootResults;
        Visible = true;

        if (TitleLabel != null)
            TitleLabel.Text = sourceName;

        RefreshDisplay();
    }

    private void RefreshDisplay()
    {
        if (LootListContainer == null) return;

        foreach (var child in LootListContainer.GetChildren())
        {
            child.QueueFree();
        }

        for (int i = 0; i < _currentLoot.Count; i++)
        {
            var result = _currentLoot[i];
            var button = new Button();

            if (result.IsGold)
            {
                button.Text = $"💰 金币 x{result.GoldAmount}";
            }
            else if (result.Item != null)
            {
                button.Text = $"{result.Item.ItemName} x{result.Quantity}";
            }

            int capturedIndex = i;
            button.Pressed += () => OnTakeLoot(capturedIndex);
            LootListContainer.AddChild(button);
        }
    }

    private void OnTakeLoot(int index)
    {
        if (index >= 0 && index < _currentLoot.Count)
        {
            EmitSignal(SignalName.LootTaken, index);
            _currentLoot.RemoveAt(index);
            RefreshDisplay();

            if (_currentLoot.Count == 0) Close();
        }
    }

    private void OnTakeAll()
    {
        EmitSignal(SignalName.AllLootTaken);
        _currentLoot.Clear();
        Close();
    }

    public void Close()
    {
        Visible = false;
        _currentLoot.Clear();
    }
}

