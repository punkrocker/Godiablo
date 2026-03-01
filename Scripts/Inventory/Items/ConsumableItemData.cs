using Godot;
using Diablo.Core.Enums;

namespace Diablo.Inventory.Items;

/// <summary>
/// 消耗品物品数据
/// 如药水、食物、卷轴等
/// </summary>
[GlobalClass]
public partial class ConsumableItemData : ItemData
{
    public enum EffectType { Heal, RestoreMana, RestoreStamina, Buff, Poison, Cure }

    [Export] public EffectType Effect { get; set; } = EffectType.Heal;
    [Export] public float Magnitude { get; set; } = 50f;
    [Export] public float Duration { get; set; } = 0f; // 0表示即时效果
    [Export] public StatType AffectedStat { get; set; } = StatType.Health;

    public ConsumableItemData()
    {
        Type = ItemType.Consumable;
        IsStackable = true;
        MaxStackSize = 20;
    }
}

