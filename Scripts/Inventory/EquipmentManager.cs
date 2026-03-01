using Godot;
using System.Collections.Generic;
using Diablo.Core.Enums;
using Diablo.Core.Events;
using Diablo.Character;
using Diablo.Inventory.Items;

namespace Diablo.Inventory;

/// <summary>
/// 装备管理器
/// 管理角色的装备槽位，处理装备/卸下及属性加成
/// </summary>
public partial class EquipmentManager : Node
{
    [Export] public CharacterStats OwnerStats { get; set; }

    /// <summary>装备槽位字典</summary>
    public Dictionary<EquipmentSlot, ItemData> EquippedItems { get; private set; } = new();

    [Signal] public delegate void EquipmentChangedEventHandler(int slot);

    public override void _Ready()
    {
        // 初始化所有槽位为空
        foreach (EquipmentSlot slot in System.Enum.GetValues(typeof(EquipmentSlot)))
        {
            EquippedItems[slot] = null;
        }
    }

    /// <summary>
    /// 装备物品
    /// </summary>
    public bool Equip(ItemData item, EquipmentSlot slot)
    {
        if (item == null) return false;

        // 如果该槽位已有装备，先卸下
        if (EquippedItems[slot] != null)
        {
            Unequip(slot);
        }

        EquippedItems[slot] = item;
        ApplyItemStats(item, true);

        EmitSignal(SignalName.EquipmentChanged, (int)slot);
        GameEvents.EmitItemEquipped(item.ItemId, slot);

        return true;
    }

    /// <summary>
    /// 卸下装备
    /// </summary>
    public ItemData Unequip(EquipmentSlot slot)
    {
        var item = EquippedItems[slot];
        if (item == null) return null;

        ApplyItemStats(item, false);
        EquippedItems[slot] = null;

        EmitSignal(SignalName.EquipmentChanged, (int)slot);
        GameEvents.EmitItemUnequipped(item.ItemId, slot);

        return item;
    }

    /// <summary>
    /// 获取指定槽位的装备
    /// </summary>
    public ItemData GetEquippedItem(EquipmentSlot slot)
    {
        return EquippedItems.GetValueOrDefault(slot, null);
    }

    /// <summary>
    /// 应用/移除装备属性加成
    /// </summary>
    private void ApplyItemStats(ItemData item, bool isEquipping)
    {
        if (OwnerStats == null) return;

        float modifier = isEquipping ? 1f : -1f;

        if (item is ArmorItemData armor)
        {
            OwnerStats.BaseArmor += armor.DefenseValue * modifier;

            foreach (var resistance in armor.ResistanceBonus)
            {
                if (OwnerStats.Resistances.ContainsKey(resistance.Key))
                {
                    OwnerStats.Resistances[resistance.Key] += resistance.Value * modifier;
                }
            }
        }
    }

    /// <summary>
    /// 计算所有装备的总防御值
    /// </summary>
    public float GetTotalArmor()
    {
        float total = 0f;
        foreach (var kvp in EquippedItems)
        {
            if (kvp.Value is ArmorItemData armor)
            {
                total += armor.DefenseValue;
            }
        }
        return total;
    }
}

