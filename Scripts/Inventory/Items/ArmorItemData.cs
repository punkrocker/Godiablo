using Godot;
using Diablo.Core.Enums;
using System.Collections.Generic;

namespace Diablo.Inventory.Items;

/// <summary>
/// 护甲物品数据
/// 包含防御值和元素抗性
/// </summary>
[GlobalClass]
public partial class ArmorItemData : ItemData
{
    [Export] public float DefenseValue { get; set; } = 5f;
    [Export] public EquipmentSlot Slot { get; set; } = EquipmentSlot.Chest;

    /// <summary>元素抗性加成 (DamageType => 抗性值 0~1)</summary>
    public Dictionary<DamageType, float> ResistanceBonus { get; set; } = new();

    public ArmorItemData()
    {
        Type = ItemType.Armor;
        IsStackable = false;
        MaxStackSize = 1;
    }
}

