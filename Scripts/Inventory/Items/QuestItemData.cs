using Godot;

namespace Diablo.Inventory.Items;

/// <summary>
/// 任务物品数据
/// 与任务关联的特殊物品，通常不可丢弃
/// </summary>
[GlobalClass]
public partial class QuestItemData : ItemData
{
    [Export] public string RelatedQuestId { get; set; } = "";
    [Export] public bool IsDroppable { get; set; } = false;

    public QuestItemData()
    {
        Type = Core.Enums.ItemType.QuestItem;
        IsStackable = false;
        MaxStackSize = 1;
    }
}

