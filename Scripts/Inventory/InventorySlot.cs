using Diablo.Inventory.Items;

namespace Diablo.Inventory;

/// <summary>
/// 背包槽位
/// 存储物品引用和数量
/// </summary>
public class InventorySlot
{
    /// <summary>物品数据</summary>
    public ItemData Item { get; set; }

    /// <summary>当前数量</summary>
    public int Quantity { get; set; }

    public bool IsEmpty => Item == null || Quantity <= 0;

    public InventorySlot() { }

    public InventorySlot(ItemData item, int quantity = 1)
    {
        Item = item;
        Quantity = quantity;
    }

    /// <summary>
    /// 尝试添加数量
    /// </summary>
    public int AddQuantity(int amount)
    {
        if (Item == null) return amount;

        int canAdd = Item.MaxStackSize - Quantity;
        int toAdd = amount > canAdd ? canAdd : amount;
        Quantity += toAdd;
        return amount - toAdd; // 返回未添加的剩余数量
    }

    /// <summary>
    /// 移除数量
    /// </summary>
    public bool RemoveQuantity(int amount)
    {
        if (Quantity < amount) return false;
        Quantity -= amount;
        if (Quantity <= 0)
        {
            Item = null;
            Quantity = 0;
        }
        return true;
    }
}

