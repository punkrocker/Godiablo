using Godot;
using Diablo.Core.Enums;

namespace Diablo.Inventory.Items;

/// <summary>
/// 物品数据基类 - 所有物品的抽象基类
/// 作为 Godot Resource 使用，可在编辑器中创建和配置
/// </summary>
[GlobalClass]
public partial class ItemData : Resource
{
    [Export] public string ItemId { get; set; } = "";
    [Export] public string ItemName { get; set; } = "未命名物品";
    [Export(PropertyHint.MultilineText)] public string Description { get; set; } = "";
    [Export] public Texture2D Icon { get; set; }
    [Export] public float Weight { get; set; } = 1f;
    [Export] public int Value { get; set; } = 10;
    [Export] public ItemType Type { get; set; } = ItemType.Misc;
    [Export] public ItemRarity Rarity { get; set; } = ItemRarity.Common;
    [Export] public bool IsStackable { get; set; } = false;
    [Export] public int MaxStackSize { get; set; } = 1;
}

