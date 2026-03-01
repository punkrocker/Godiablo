using Godot;
using Diablo.Core.Enums;
using Diablo.Combat.Weapon;

namespace Diablo.Inventory.Items;

/// <summary>
/// 武器物品数据
/// 继承 ItemData，增加武器特定属性
/// </summary>
[GlobalClass]
public partial class WeaponItemData : ItemData
{
    [Export] public WeaponData WeaponStats { get; set; }
    [Export] public EquipmentSlot Slot { get; set; } = EquipmentSlot.MainHand;

    public WeaponItemData()
    {
        Type = ItemType.Weapon;
        IsStackable = false;
        MaxStackSize = 1;
    }
}

