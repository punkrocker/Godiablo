using Diablo.Core.Enums;
using Diablo.Inventory.Items;

namespace Diablo.Loot;

/// <summary>
/// 战利品条目
/// 定义单个可掉落物品的信息
/// </summary>
public class LootEntry
{
    /// <summary>物品数据</summary>
    public ItemData Item { get; set; }

    /// <summary>物品ID（用于从数据库加载）</summary>
    public string ItemId { get; set; } = "";

    /// <summary>掉落概率 (0~1)</summary>
    public float DropChance { get; set; } = 0.5f;

    /// <summary>最小掉落数量</summary>
    public int MinQuantity { get; set; } = 1;

    /// <summary>最大掉落数量</summary>
    public int MaxQuantity { get; set; } = 1;

    /// <summary>稀有度权重（用于加权随机）</summary>
    public float RarityWeight { get; set; } = 1f;
}

