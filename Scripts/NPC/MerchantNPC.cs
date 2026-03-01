using Godot;
using System.Collections.Generic;
using Diablo.Inventory;
using Diablo.Inventory.Items;

namespace Diablo.NPC;

/// <summary>
/// 商人NPC
/// 扩展 NPCBase，提供买卖功能
/// </summary>
public partial class MerchantNPC : NPCBase
{
    [Export] public float BuyPriceMultiplier { get; set; } = 1.0f;
    [Export] public float SellPriceMultiplier { get; set; } = 0.5f;

    /// <summary>商店库存</summary>
    public List<InventorySlot> ShopInventory { get; set; } = new();

    /// <summary>商人金币</summary>
    [Export] public int Gold { get; set; } = 1000;

    [Signal] public delegate void ShopOpenedEventHandler(string merchantId);
    [Signal] public delegate void ShopClosedEventHandler();

    public override void _Ready()
    {
        base._Ready();
        AddToGroup("Merchant");
    }

    public override void OnInteract(Node interactor)
    {
        base.OnInteract(interactor);
        OpenShop();
    }

    /// <summary>
    /// 打开商店界面
    /// </summary>
    public void OpenShop()
    {
        EmitSignal(SignalName.ShopOpened, NpcId);
    }

    /// <summary>
    /// 关闭商店
    /// </summary>
    public void CloseShop()
    {
        EmitSignal(SignalName.ShopClosed);
        EndInteraction();
    }

    /// <summary>
    /// 计算购买价格
    /// </summary>
    public int GetBuyPrice(ItemData item)
    {
        return (int)(item.Value * BuyPriceMultiplier);
    }

    /// <summary>
    /// 计算出售价格
    /// </summary>
    public int GetSellPrice(ItemData item)
    {
        return (int)(item.Value * SellPriceMultiplier);
    }

    /// <summary>
    /// 玩家从商人处购买物品
    /// </summary>
    public bool BuyItem(int shopSlotIndex, Inventory.Inventory playerInventory, ref int playerGold)
    {
        if (shopSlotIndex < 0 || shopSlotIndex >= ShopInventory.Count) return false;

        var slot = ShopInventory[shopSlotIndex];
        if (slot.Item == null) return false;

        int price = GetBuyPrice(slot.Item);
        if (playerGold < price) return false;

        if (!playerInventory.AddItem(slot.Item, 1)) return false;

        playerGold -= price;
        Gold += price;
        slot.RemoveQuantity(1);

        if (slot.Quantity <= 0)
        {
            ShopInventory.RemoveAt(shopSlotIndex);
        }

        return true;
    }

    /// <summary>
    /// 玩家向商人出售物品
    /// </summary>
    public bool SellItem(ItemData item, Inventory.Inventory playerInventory, ref int playerGold)
    {
        if (item == null) return false;

        int price = GetSellPrice(item);
        if (Gold < price) return false;

        if (!playerInventory.RemoveItem(item.ItemId, 1)) return false;

        playerGold += price;
        Gold -= price;

        return true;
    }
}

