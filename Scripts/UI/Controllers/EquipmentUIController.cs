using Godot;
using Diablo.Core.Enums;
using Diablo.Inventory;

namespace Diablo.UI.Controllers;

/// <summary>
/// 装备UI控制器
/// 显示角色纸娃娃和已装备物品
/// </summary>
public partial class EquipmentUIController : Control
{
    [Export] public EquipmentManager Equipment { get; set; }

    // 各槽位UI引用
    [Export] public TextureRect HeadSlot { get; set; }
    [Export] public TextureRect ChestSlot { get; set; }
    [Export] public TextureRect LegsSlot { get; set; }
    [Export] public TextureRect FeetSlot { get; set; }
    [Export] public TextureRect HandsSlot { get; set; }
    [Export] public TextureRect MainHandSlot { get; set; }
    [Export] public TextureRect OffHandSlot { get; set; }
    [Export] public TextureRect RingSlot { get; set; }
    [Export] public TextureRect AmuletSlot { get; set; }

    [Export] public Label ArmorValueLabel { get; set; }

    public override void _Ready()
    {
        if (Equipment != null)
        {
            Equipment.EquipmentChanged += OnEquipmentChanged;
        }
        RefreshDisplay();
    }

    private void OnEquipmentChanged(int slot)
    {
        RefreshDisplay();
    }

    public void RefreshDisplay()
    {
        if (Equipment == null) return;

        UpdateSlotDisplay(HeadSlot, EquipmentSlot.Head);
        UpdateSlotDisplay(ChestSlot, EquipmentSlot.Chest);
        UpdateSlotDisplay(LegsSlot, EquipmentSlot.Legs);
        UpdateSlotDisplay(FeetSlot, EquipmentSlot.Feet);
        UpdateSlotDisplay(HandsSlot, EquipmentSlot.Hands);
        UpdateSlotDisplay(MainHandSlot, EquipmentSlot.MainHand);
        UpdateSlotDisplay(OffHandSlot, EquipmentSlot.OffHand);
        UpdateSlotDisplay(RingSlot, EquipmentSlot.Ring);
        UpdateSlotDisplay(AmuletSlot, EquipmentSlot.Amulet);

        if (ArmorValueLabel != null)
        {
            ArmorValueLabel.Text = $"护甲: {Equipment.GetTotalArmor():F0}";
        }
    }

    private void UpdateSlotDisplay(TextureRect slotUI, EquipmentSlot slot)
    {
        if (slotUI == null) return;

        var item = Equipment.GetEquippedItem(slot);
        slotUI.Texture = item?.Icon;
    }
}

