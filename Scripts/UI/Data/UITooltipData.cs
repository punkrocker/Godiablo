using Diablo.Core.Enums;

namespace Diablo.UI.Data;

/// <summary>
/// UI工具提示数据
/// </summary>
public class UITooltipData
{
    public string Title { get; set; } = "";
    public string[] DescriptionLines { get; set; } = System.Array.Empty<string>();
    public string[] StatModifiers { get; set; } = System.Array.Empty<string>();
    public ItemRarity Rarity { get; set; } = ItemRarity.Common;
    public string TypeLabel { get; set; } = "";
    public int Value { get; set; } = 0;
    public float Weight { get; set; } = 0f;
}

