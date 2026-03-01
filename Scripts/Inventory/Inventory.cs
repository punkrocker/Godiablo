using Godot;
using System.Collections.Generic;
using System.Linq;
using Diablo.Core.Events;
using Diablo.Inventory.Items;

namespace Diablo.Inventory;

/// <summary>
/// 背包系统
/// 管理物品的添加、移除、排序和搜索
/// </summary>
public partial class Inventory : Node
{
    [Export] public int MaxSlots { get; set; } = 40;
    [Export] public float MaxWeight { get; set; } = 300f;

    public List<InventorySlot> Slots { get; private set; } = new();
    public float CurrentWeight { get; private set; } = 0f;

    [Signal] public delegate void InventoryChangedEventHandler();
    [Signal] public delegate void ItemAddedEventHandler(string itemId, int quantity);
    [Signal] public delegate void ItemRemovedEventHandler(string itemId, int quantity);
    [Signal] public delegate void InventoryFullEventHandler();

    public override void _Ready()
    {
        // 初始化空槽位
        for (int i = 0; i < MaxSlots; i++)
        {
            Slots.Add(new InventorySlot());
        }
    }

    /// <summary>
    /// 添加物品到背包
    /// </summary>
    public bool AddItem(ItemData item, int quantity = 1)
    {
        if (item == null || quantity <= 0) return false;

        // 检查重量
        if (CurrentWeight + item.Weight * quantity > MaxWeight) return false;

        int remaining = quantity;

        // 先尝试堆叠到现有槽位
        if (item.IsStackable)
        {
            foreach (var slot in Slots)
            {
                if (slot.Item?.ItemId == item.ItemId && slot.Quantity < item.MaxStackSize)
                {
                    remaining = slot.AddQuantity(remaining);
                    if (remaining <= 0) break;
                }
            }
        }

        // 放到空槽位
        while (remaining > 0)
        {
            var emptySlot = Slots.FirstOrDefault(s => s.IsEmpty);
            if (emptySlot == null)
            {
                EmitSignal(SignalName.InventoryFull);
                return false;
            }

            int toAdd = item.IsStackable ? System.Math.Min(remaining, item.MaxStackSize) : 1;
            emptySlot.Item = item;
            emptySlot.Quantity = toAdd;
            remaining -= toAdd;
        }

        CurrentWeight += item.Weight * quantity;
        EmitSignal(SignalName.ItemAdded, item.ItemId, quantity);
        EmitSignal(SignalName.InventoryChanged);
        GameEvents.EmitItemPickedUp(item.ItemId, quantity);

        return true;
    }

    /// <summary>
    /// 从背包移除物品
    /// </summary>
    public bool RemoveItem(string itemId, int quantity = 1)
    {
        int remaining = quantity;

        foreach (var slot in Slots)
        {
            if (slot.Item?.ItemId == itemId)
            {
                int canRemove = System.Math.Min(remaining, slot.Quantity);
                slot.RemoveQuantity(canRemove);
                remaining -= canRemove;

                if (slot.Item != null)
                {
                    CurrentWeight -= slot.Item.Weight * canRemove;
                }

                if (remaining <= 0) break;
            }
        }

        if (remaining < quantity)
        {
            EmitSignal(SignalName.ItemRemoved, itemId, quantity - remaining);
            EmitSignal(SignalName.InventoryChanged);
            return true;
        }

        return false;
    }

    /// <summary>
    /// 查询物品数量
    /// </summary>
    public int GetItemCount(string itemId)
    {
        return Slots.Where(s => s.Item?.ItemId == itemId).Sum(s => s.Quantity);
    }

    /// <summary>
    /// 检查是否拥有足够数量的物品
    /// </summary>
    public bool HasItem(string itemId, int quantity = 1)
    {
        return GetItemCount(itemId) >= quantity;
    }

    /// <summary>
    /// 按稀有度排序
    /// </summary>
    public void SortByRarity()
    {
        var items = Slots.Where(s => !s.IsEmpty).OrderByDescending(s => s.Item.Rarity).ToList();
        for (int i = 0; i < MaxSlots; i++)
        {
            Slots[i] = i < items.Count ? items[i] : new InventorySlot();
        }
        EmitSignal(SignalName.InventoryChanged);
    }

    /// <summary>
    /// 按类型排序
    /// </summary>
    public void SortByType()
    {
        var items = Slots.Where(s => !s.IsEmpty).OrderBy(s => s.Item.Type).ThenBy(s => s.Item.ItemName).ToList();
        for (int i = 0; i < MaxSlots; i++)
        {
            Slots[i] = i < items.Count ? items[i] : new InventorySlot();
        }
        EmitSignal(SignalName.InventoryChanged);
    }

    /// <summary>
    /// 搜索物品
    /// </summary>
    public List<InventorySlot> SearchItems(string keyword)
    {
        return Slots.Where(s => !s.IsEmpty &&
            (s.Item.ItemName.Contains(keyword) || s.Item.Description.Contains(keyword)))
            .ToList();
    }
}

