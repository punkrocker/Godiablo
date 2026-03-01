using Godot;
using Diablo.Core.Enums;

namespace Diablo.UI.Data;

/// <summary>
/// UI物品槽数据 - 用于在UI中显示物品信息
/// </summary>
public class UIItemSlotData
{
    public string ItemId { get; set; } = "";
    public string ItemName { get; set; } = "";
    public Texture2D Icon { get; set; }
    public int Quantity { get; set; } = 1;
    public ItemRarity Rarity { get; set; } = ItemRarity.Common;
    public bool IsEmpty { get; set; } = true;

    /// <summary>
    /// 根据稀有度获取颜色
    /// </summary>
    public Color GetRarityColor()
    {
        return Rarity switch
        {
            ItemRarity.Common => Colors.White,
            ItemRarity.Uncommon => Colors.Green,
            ItemRarity.Rare => Colors.Blue,
            ItemRarity.Epic => Colors.Purple,
            ItemRarity.Legendary => Colors.Orange,
            ItemRarity.Daedric => Colors.Red,
            _ => Colors.White
        };
    }
}

