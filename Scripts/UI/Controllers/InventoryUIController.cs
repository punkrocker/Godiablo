﻿using Godot;
using System.Collections.Generic;
using Diablo.Inventory;
using Diablo.UI.Data;

namespace Diablo.UI.Controllers;

/// <summary>
/// 背包UI控制器
/// 渲染物品格子，处理拖拽、右键菜单（使用/装备/丢弃）
/// </summary>
public partial class InventoryUIController : Control
{
    [Export] public Inventory.Inventory PlayerInventory { get; set; }
    [Export] public GridContainer ItemGrid { get; set; }
    [Export] public PackedScene ItemSlotScene { get; set; }
    [Export] public Label WeightLabel { get; set; }

    private List<UIItemSlotData> _displayData = new();

    [Signal] public delegate void ItemSlotClickedEventHandler(int slotIndex, int mouseButton);
    [Signal] public delegate void ItemSlotHoveredEventHandler(int slotIndex);

    public override void _Ready()
    {
        if (PlayerInventory != null)
        {
            PlayerInventory.InventoryChanged += RefreshDisplay;
        }
        Visible = false;
    }

    public override void _Input(InputEvent @event)
    {
        if (InputMap.HasAction("open_inventory") && @event.IsActionPressed("open_inventory"))
        {
            ToggleVisibility();
        }
    }

    public void ToggleVisibility()
    {
        Visible = !Visible;
        if (Visible)
        {
            RefreshDisplay();
        }
    }

    /// <summary>
    /// 刷新背包显示
    /// </summary>
    public void RefreshDisplay()
    {
        if (PlayerInventory == null || ItemGrid == null) return;

        _displayData.Clear();

        // 清空网格
        foreach (var child in ItemGrid.GetChildren())
        {
            child.QueueFree();
        }

        // 根据背包数据填充
        for (int i = 0; i < PlayerInventory.Slots.Count; i++)
        {
            var slot = PlayerInventory.Slots[i];
            var displaySlot = new UIItemSlotData();

            if (!slot.IsEmpty)
            {
                displaySlot.ItemId = slot.Item.ItemId;
                displaySlot.ItemName = slot.Item.ItemName;
                displaySlot.Icon = slot.Item.Icon;
                displaySlot.Quantity = slot.Quantity;
                displaySlot.Rarity = slot.Item.Rarity;
                displaySlot.IsEmpty = false;
            }

            _displayData.Add(displaySlot);

            // 创建槽位UI节点
            if (ItemSlotScene != null)
            {
                var slotNode = ItemSlotScene.Instantiate<Control>();
                ItemGrid.AddChild(slotNode);
            }
        }

        // 更新重量
        if (WeightLabel != null)
        {
            WeightLabel.Text = $"重量: {PlayerInventory.CurrentWeight:F1}/{PlayerInventory.MaxWeight:F1}";
        }
    }
}

