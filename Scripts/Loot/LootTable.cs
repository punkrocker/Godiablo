using Godot;
using System.Collections.Generic;
using Diablo.Core.Helpers;
using Diablo.Inventory.Items;
using Diablo.Inventory;

namespace Diablo.Loot;

/// <summary>
/// 战利品表资源
/// 定义一组可掉落物品及其概率
/// </summary>
[GlobalClass]
public partial class LootTable : Resource
{
    [Export] public string TableId { get; set; } = "";

    /// <summary>掉落条目列表</summary>
    public List<LootEntry> Entries { get; set; } = new();

    /// <summary>保底掉落物品数量</summary>
    [Export] public int GuaranteedDropCount { get; set; } = 1;

    /// <summary>最大额外掉落数量</summary>
    [Export] public int MaxExtraDrops { get; set; } = 3;

    /// <summary>金币掉落范围</summary>
    [Export] public int MinGold { get; set; } = 0;
    [Export] public int MaxGold { get; set; } = 50;

    /// <summary>
    /// 根据概率和权重掷骰，返回掉落的物品列表
    /// </summary>
    public List<LootResult> Roll()
    {
        var results = new List<LootResult>();

        // 金币掉落
        if (MaxGold > 0)
        {
            int gold = MathUtils.RandRange(MinGold, MaxGold);
            if (gold > 0)
            {
                results.Add(new LootResult { IsGold = true, GoldAmount = gold });
            }
        }

        // 保底掉落（从列表中按权重随机选取）
        for (int i = 0; i < GuaranteedDropCount && Entries.Count > 0; i++)
        {
            var entry = WeightedSelect();
            if (entry?.Item != null)
            {
                int qty = MathUtils.RandRange(entry.MinQuantity, entry.MaxQuantity);
                results.Add(new LootResult { Item = entry.Item, Quantity = qty });
            }
        }

        // 额外掉落（逐条判定概率）
        int extraCount = 0;
        foreach (var entry in Entries)
        {
            if (extraCount >= MaxExtraDrops) break;

            if (MathUtils.RollChance(entry.DropChance) && entry.Item != null)
            {
                int qty = MathUtils.RandRange(entry.MinQuantity, entry.MaxQuantity);
                results.Add(new LootResult { Item = entry.Item, Quantity = qty });
                extraCount++;
            }
        }

        return results;
    }

    /// <summary>
    /// 在世界中生成掉落物
    /// </summary>
    public void DropLoot(Vector3 position, SceneTree tree)
    {
        var results = Roll();
        foreach (var result in results)
        {
            if (result.IsGold)
            {
                GD.Print($"[掉落] 金币 x{result.GoldAmount}");
            }
            else if (result.Item != null)
            {
                GD.Print($"[掉落] {result.Item.ItemName} x{result.Quantity}");
                // 可以在此处实例化世界中的掉落物场景
            }
        }
    }

    private LootEntry WeightedSelect()
    {
        if (Entries.Count == 0) return null;

        float[] weights = new float[Entries.Count];
        for (int i = 0; i < Entries.Count; i++)
        {
            weights[i] = Entries[i].RarityWeight;
        }

        int index = MathUtils.WeightedRandomIndex(weights);
        return Entries[index];
    }
}

/// <summary>
/// 掉落结果
/// </summary>
public class LootResult
{
    public ItemData Item { get; set; }
    public int Quantity { get; set; } = 1;
    public bool IsGold { get; set; } = false;
    public int GoldAmount { get; set; } = 0;
}

